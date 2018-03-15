
open System
open System.IO
open System.Net
open imdbDownload
open imdbProviders

open Microsoft.FSharp.Data;
open Microsoft.FSharp.Linq;
open imdbSeed;


[<EntryPoint>]
let main argv = 

    let fileNames = [MediaFile; MediaRatingFile; (*EpisodesFile;*) StaffFile; (*CrewFile;*) PrincipalsFile ]

    //download and unzip imdb data files
    ignore(List.map downloadIMDBFile fileNames)
    ignore(List.map (fun x -> (unzipIMDBFile x (Path.GetFileNameWithoutExtension x.Value))) fileNames)
    printfn "Done Downloading and unzipping IMDB Files"
                          
    printfn "Seeding Media"
    seedMedia()
                  
    printfn "Seeding Ratings"
    seedRatings()

    printfn "Seeding Staff"
    seedStaff()

    printfn "Seeding Principal"
    seedPrincipals()

    printfn "all done seeding the db!"

    0
