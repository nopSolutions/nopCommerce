namespace Nop.Services.Media.RoxyFileman;

public partial record RoxyFileInfo(string RelativePath, DateTimeOffset LastWriteTime, long FileLength, int Width, int Height);
public partial record RoxyDirectoryInfo(string RelativePath, int CountFiles, int CountDirectories);