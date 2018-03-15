
module imdbDownload

    open System
    open System.IO
    open System.Net
    open System.IO.Compression

    //later on, put url in config file
    let urlBase = "https://datasets.imdbws.com"

    type MediaFiles =
        | MediaFile
        | MediaRatingFile
        | EpisodesFile
        | StaffFile
        | CrewFile
        | PrincipalsFile
        member this.Value =
            match this with
            | MediaFile -> "title.basics.tsv.gz"
            | MediaRatingFile -> "title.ratings.tsv.gz"
            | EpisodesFile -> "title.episode.tsv.gz"
            | StaffFile -> "name.basics.tsv.gz"
            | CrewFile -> "title.crew.tsv.gz"
            | PrincipalsFile -> "title.principals.tsv.gz"
        member this.Path =
            String.concat "/" [Environment.CurrentDirectory; "rawData"; Path.GetFileNameWithoutExtension(this.Value)]

    let internal createRawDataFolder() =
        if (not (System.IO.Directory.Exists("rawData"))) then
            ignore(Directory.CreateDirectory("rawData"))
        ()

    let downloadIMDBFile (fileName : MediaFiles) =
        printfn "Downloading file %s from %s" fileName.Value urlBase
        createRawDataFolder()
        let wc = new WebClient()
        let address = String.concat @"/" [urlBase; fileName.Value]
        let localPath = String.concat @"/" ["rawData" ; fileName.Value]
        wc.DownloadFile(address, localPath)
        ()

    let unzipIMDBFile (fileName : MediaFiles) decompressedFileName = 
        printfn "Unzipping %s" fileName.Value
        createRawDataFolder()
        let currentDirectory = Environment.CurrentDirectory;
        let fileName = String.concat "/" [currentDirectory ; "rawData" ; fileName.Value]
        let gzipFileInfo = new FileInfo(fileName)
        use fileToDecompressAsStream = gzipFileInfo.OpenRead()
        let decompressedFileName = String.concat "/" [currentDirectory ; "rawData" ; decompressedFileName]
        use decompressedStream  = File.Create(decompressedFileName)
        use decompressionStream = new GZipStream(fileToDecompressAsStream, CompressionMode.Decompress)
        try
            decompressionStream.CopyTo(decompressedStream);
            System.IO.File.Delete(fileName)
        with
        | _ -> printf "Error decompressing file %s" fileName
        ()