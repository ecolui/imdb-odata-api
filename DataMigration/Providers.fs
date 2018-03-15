namespace imdbProviders

    open FSharp.Data

    //IgnoreErrors was set to true because there was an instance where a row had 8 columns instead of 9
    //Also, type provider looks at the first 1000k records by default to infer types. For isAdult, it'll infer int. But
    //further down in the dataset, there are a few "/N", which would suggest that this is a null value
    type mediaProvider = CsvProvider<"tconst\ttitleType\tprimaryTitle\toriginalTitle\tisAdult\tstartYear\tendYear\truntimeMinutes\tgenres", IgnoreErrors=true, Schema=",,,,isAdult (string), startYear (string), endYear (string)" >

    type mediaRatingProvider = CsvProvider<"tconst\taverageRating\tnumVotes", Separators="\t", HasHeaders = true, Schema = "tconst (string), averageRating (decimal), numVotes (int64)">

    type staffProvider = CsvProvider<"nconst\tprimaryName\tbirthYear\tdeathYear\tprimaryProfession\tknownForTitles", HasHeaders = true>

    type principalsProvider = CsvProvider<"tconst\tordering\tnconst\tcategory\tjob\tcharacters", HasHeaders = true, IgnoreErrors=true>

