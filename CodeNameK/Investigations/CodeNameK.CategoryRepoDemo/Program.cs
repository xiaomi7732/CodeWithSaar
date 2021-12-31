using CodeWithSaar;

const string Data = "Data";
void CreateCategory(string categoryName)
{
    string targetPath = Path.Combine(Data, FileUtility.Encode(categoryName));
    Directory.CreateDirectory(targetPath);
}

CreateCategory(args[0]);