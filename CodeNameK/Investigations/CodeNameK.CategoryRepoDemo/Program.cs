const string Data = "Data";
void CreateCategory(string categoryName)
{
    string targetPath = Path.Combine(Data, categoryName);
    Directory.CreateDirectory(targetPath);
}

CreateCategory(args[0]);