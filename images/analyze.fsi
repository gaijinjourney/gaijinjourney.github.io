open System
open System.IO

let basePath = @"C:\Users\anthy\Dropbox\Public\blog\voyage\japon"
let baseWebPath = "http://dl.dropboxusercontent.com/u/44389563/blog/voyage/japon/"
type FileType = |Video|Photo|Other
type File = {Path:string;FolderPath:string;Type:FileType}

let rec getRecursivlyFolders path = 
    let getPath (file:FileSystemInfo) = file.FullName
    let directory = new DirectoryInfo(path)
    let subDirectories  = directory.GetDirectories()|> Seq.map getPath
    seq {yield! subDirectories; for d in subDirectories do yield! getRecursivlyFolders d}



let toFile (folderPath:string) (filePath:FileInfo) = 
    match filePath with
    | f when f.Extension.ToLower() = ".jpg" -> {Path = f.FullName;FolderPath = folderPath;Type = Photo }
    | f when f.Extension.ToLower() = ".mp4" -> {Path = f.FullName;FolderPath = folderPath;Type = Video }
    | f -> {Path = f.FullName; FolderPath = folderPath; Type = Other }


let getFiles folderPath = (new DirectoryInfo(folderPath)).GetFiles() |> Seq.map (toFile folderPath)

let relativePath (file:File) = file.Path.Substring(basePath.Length + 1).Replace('\\', '/')

let toHtml (file:File) =
    let webPath = baseWebPath + (relativePath file)
    match file.Type with 
    | Photo -> file.FolderPath, sprintf  @"<img src=""%s"" alt="""" />" webPath
    | Video -> file.FolderPath,  sprintf @"[video src=""%s""]" webPath
    |  _ -> file.FolderPath, ""

let file = {Path = @"C:\Users\anthy\Dropbox\Public\blog\voyage\japon\j2\tour\DSC00986.jpg"; FolderPath = @"C:\Users\anthy\Dropbox\Public\blog\voyage\japon\j2\tour"; Type = Photo}

let write (htmls:(string*string) list) = 
    if htmls.Length > 0 then
        let folderPath, x = List.head htmls
        let lines = htmls |> Seq.map (fun (x, html) -> html) |> Seq.toArray
        File.WriteAllLines(folderPath + "\\scripts.txt", lines)


getRecursivlyFolders basePath 
|> Seq.map getFiles 
|> Seq.map (fun files -> Seq.map toHtml files |> Seq.toList)
|> Seq.iter write
