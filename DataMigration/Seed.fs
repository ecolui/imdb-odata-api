
module imdbSeed

    open imdbDownload
    open imdbProviders
    open DataAccess

    let parseInt value : System.Nullable<int> = 
        match value with
            | "\N" -> new System.Nullable<int>()
            | value -> 
                match System.Int32.TryParse(value) with
                    | (true, value) -> new System.Nullable<int>(value)
                    | _ -> new System.Nullable<int>()

    let parseBool value : System.Nullable<bool> = 
        match value with
            | "\N" -> new System.Nullable<bool>()
            | value -> 
                match System.Int16.TryParse(value) with
                    | (true, value) -> 
                        match value with
                            | 1s -> new System.Nullable<bool>(true)
                            | _ -> new System.Nullable<bool>(false)
                    | _ -> new System.Nullable<bool>()    

    let parseString value =
        match value with
            | "\N" -> ""
            | _ -> value                

    type internal DbContextBatcher(maxBatchItems: int, ctx: imdbContext) =
        let mutable count = 0
        let mutable batch = 0
        let mutable itemsInBatch = maxBatchItems
        member this.tryBatchSave() =
            count <-
                if (count = itemsInBatch) then
                    ignore(ctx.SaveChanges())
                    batch <- batch + 1
                    printfn "Saved Batch %i" batch
                    0
                else
                    count + 1
        member this.DisableChangeTracker() =
            ctx.ChangeTracker.AutoDetectChangesEnabled <- false
        member this.save() =
            printfn "Saving Data"
            ignore(ctx.SaveChanges())
        member this.AddEntity entity =
            ignore(ctx.Add(entity))

    let seedMedia() = 
        let media = mediaProvider.Load(MediaFile.Path)
        let ctxFactory = new imdbContextFactory()
        let ctx = ctxFactory.CreateDbContext(Array.empty<string>)
        let dataPersist = new DbContextBatcher(1000, ctx)

        //turn off change tracker in order to speed things up. We don't need it, since
        //we are only adding new entries to the database, not mutating them.
        dataPersist.DisableChangeTracker()

        //To simplify things for now, narrow the scope to movies (no episodes) after 2000 with at least 1000 ratings
        let mediaWithRatings =
            let ratings = mediaRatingProvider.Load(MediaRatingFile.Path)
            ratings.Rows
                |> Seq.filter(fun m -> m.NumVotes > 1000L)
                |> Seq.map (fun m -> m.Tconst)
                |> Seq.toArray

        media.Rows
            |> Seq.filter (fun m -> 
                    let startYear = parseInt m.StartYear
                    let titleType = m.TitleType
                    if (startYear.HasValue && startYear.Value >= 2000 && titleType = "movie") then
                        true
                    else
                        false
                )
            //use Seq.iter to force eager evaluation. The side effect (writing to db) should occur immediately.
            |> Seq.iter (fun m ->                
                let includeMedia =
                    match Array.tryFindIndex (fun mediaWithRating -> m.Tconst = mediaWithRating) mediaWithRatings with
                        | Some(_) -> true
                        | None -> false

                if (includeMedia = true) then
                    let entity = new Entities.Media()
                    entity.MediaIMDBId <- m.Tconst
                    entity.titleType <- m.TitleType
                    entity.primaryTitle <- m.PrimaryTitle
                    entity.isAdult <- parseBool m.IsAdult
                    entity.startYear <- parseInt m.StartYear
                    entity.endYear <- parseInt m.EndYear
                    entity.runtimeMinutes <- parseInt m.RuntimeMinutes
                    ignore(dataPersist.AddEntity(entity))

                    m.Genres.Split [|','|]
                        |> Seq.iter (fun g -> 
                            let genreEntity = new Entities.Genre()
                            genreEntity.MediaIMDBId <- m.Tconst
                            genreEntity.Description <- g
                            genreEntity.AssociatedMedia <- entity
                            ignore(dataPersist.AddEntity(genreEntity))
                            )                      
                    ignore(dataPersist.tryBatchSave())
                )
        ignore(dataPersist.save())
        printfn ("Done seeding Media")
        ()

    let seedRatings () =
        let ratings = mediaRatingProvider.Load(MediaRatingFile.Path)

        let ctxFactory = new imdbContextFactory()
        let ctx = ctxFactory.CreateDbContext(Array.empty<string>);
        let dataPersist = new DbContextBatcher(1000, ctx)
        dataPersist.DisableChangeTracker()

        let imdbMap = 
            query {
                for media in ctx.Media do
                select (media.MediaIMDBId, media)
                }
            |> Map.ofSeq

        ratings.Rows
            |> Seq.iter (fun r -> 
                match (imdbMap.TryFind r.Tconst) with
                    | None -> ()
                    | Some(media) ->
                        let ratingEntity = new Entities.MediaRating()
                        ratingEntity.AverageRating <- r.AverageRating
                        ratingEntity.MediaIMDBId <- r.Tconst
                        ratingEntity.AssociatedMedia <- media
                        ratingEntity.NumVotes <- r.NumVotes
                        dataPersist.AddEntity(ratingEntity)
                        dataPersist.tryBatchSave()
                        ()
            )

        ignore(dataPersist.save())
        printfn "Total Ratings records: %i" (Seq.length ratings.Rows)        
        ()

    let seedStaff() =
        let staff = staffProvider.Load(StaffFile.Path)

        let ctxFactory = new imdbContextFactory()
        let ctx = ctxFactory.CreateDbContext(Array.empty<string>);
        let dataPersist = new DbContextBatcher(1000, ctx)
        dataPersist.DisableChangeTracker()

        let imdbMap = 
            query {
                for media in ctx.Media do
                select (media.MediaIMDBId, media)
                }
            |> Map.ofSeq

        staff.Rows
            |> Seq.filter (fun s -> 
                //for the sake of downloading a reasonable number of files, only get actors/actresses
                s.PrimaryProfession.Contains("actor") || s.PrimaryProfession.Contains("actress")
                )
            |> Seq.iter 
                (fun s -> 
                    let popularTitles = 
                        (s.KnownForTitles.Split [|','|])
                        |> Array.filter (fun id -> imdbMap.ContainsKey(id))

                    match (Array.length popularTitles) with
                        | 0 -> ()
                        | _ ->
                            let staffEntity = new Entities.Staff()
                            staffEntity.BirthYear <- parseInt s.BirthYear
                            staffEntity.DeathYear <- parseInt s.DeathYear
                            staffEntity.imdbStaffId <- s.Nconst
                            staffEntity.PrimaryName <- s.PrimaryName
                            ignore(dataPersist.AddEntity(staffEntity))

                            popularTitles
                                |> Seq.iter (fun id -> 
                                    let knownForTitleEntity = new Entities.StaffMediaLink()
                                    knownForTitleEntity.MediaIMDBId <- id
                                    knownForTitleEntity.AssociatedMedia <- imdbMap.Item(id)
                                    knownForTitleEntity.imdbStaffId <- s.Nconst
                                    knownForTitleEntity.AssociatedStaff <- staffEntity
                                    ignore(dataPersist.AddEntity(knownForTitleEntity))
                                    )
                            
                            s.PrimaryProfession.Split [|','|]
                                |> Array.iter (fun p -> 
                                    let professionEntity = new Entities.Profession()
                                    professionEntity.Description <- p
                                    professionEntity.imdbStaffId <- s.Nconst
                                    professionEntity.AssociatedStaff <- staffEntity
                                    ignore(dataPersist.AddEntity(professionEntity))
                                    )
                            dataPersist.tryBatchSave()
                    ()
                )

        ignore(dataPersist.save())
        printfn "Total Staff records: %i" (Seq.length staff.Rows)

    let seedPrincipals() =
        let principals = principalsProvider.Load(PrincipalsFile.Path)

        let ctxFactory = new imdbContextFactory()
        let ctx = ctxFactory.CreateDbContext(Array.empty<string>);
        let dataPersist = new DbContextBatcher(1000, ctx)
        dataPersist.DisableChangeTracker()

        let imdbMap = 
            query {
                for media in ctx.Media do
                select (media.MediaIMDBId, media)
                }
            |> Map.ofSeq

        let staffIdMap = 
            query {
                for staff in ctx.Staff do
                select (staff.imdbStaffId, staff)
                }
            |> Map.ofSeq


        principals.Rows
            |> Seq.filter (fun p -> 
                //for the sake of downloading a reasonable number of files, only get actors/actresses
                p.Category = "actor" || p.Category = "actress"
                )
            |> Seq.iter (fun p -> 
                if (imdbMap.ContainsKey(p.Tconst) && staffIdMap.ContainsKey(p.Nconst)) then
                    let principalEntity = new Entities.Principal()
                    principalEntity.Category <- parseString p.Category
                    principalEntity.Characters <- parseString p.Characters
                    principalEntity.imdbStaffId <- p.Nconst
                    principalEntity.Staff <- staffIdMap.Item(p.Nconst)
                    principalEntity.Job <- parseString p.Job
                    principalEntity.MediaIMDBId <- p.Tconst
                    principalEntity.AssociatedMedia <- imdbMap.Item(p.Tconst)
                    principalEntity.Ordering <- parseInt p.Ordering
                    dataPersist.AddEntity(principalEntity)
                    dataPersist.tryBatchSave()
                    ()
                )
        ignore(dataPersist.save())
        printfn "Total Principal records: %i" (Seq.length principals.Rows)





