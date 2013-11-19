using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nop.Core.Fakes;
using Nop.Core.IO;
using NUnit.Framework;

namespace Nop.Core.Tests
{
    [TestFixture]
    public class FileSystemStorageProviderTests {

        [SetUp]
        public void Init() {
            _folderPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media"), "Default");
            _filePath = _folderPath + "\\testfile.txt";

            Directory.CreateDirectory(_folderPath);
            File.WriteAllText(_filePath, "testfile contents");

            var subfolder1 = Path.Combine(_folderPath, "Subfolder1");
            Directory.CreateDirectory(subfolder1);
            File.WriteAllText(Path.Combine(subfolder1, "one.txt"), "one contents");
            File.WriteAllText(Path.Combine(subfolder1, "two.txt"), "two contents");

            var subsubfolder1 = Path.Combine(subfolder1, "SubSubfolder1");
            Directory.CreateDirectory(subsubfolder1);

            var context = new FakeHttpContext("~/");
            var webHelper = new WebHelper(context);

            _storageProvider = new FileSystemStorageProvider(new FileSystemSettings { DirectoryName = "Default" }, webHelper);
        }

        [TearDown]
        public void Term() {
            Directory.Delete(_folderPath, true);
        }


        private string _filePath;
        private string _folderPath;
        private IStorageProvider _storageProvider;

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileThatDoesNotExistShouldThrow() {
            _storageProvider.GetFile("notexisting");
        }

        [Test]
        public void ListFilesShouldReturnFilesFromFilesystem() {
            IEnumerable<IStorageFile> files = _storageProvider.ListFiles(_folderPath);
            Assert.That(files.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ExistingFileIsReturnedWithShortPath() {
            var file = _storageProvider.GetFile("testfile.txt");
            Assert.That(file, Is.Not.Null);
            Assert.That(file.GetPath(), Is.EqualTo("testfile.txt"));
            Assert.That(file.GetName(), Is.EqualTo("testfile.txt"));
        }


        [Test]
        public void ListFilesReturnsItemsWithShortPathAndEnvironmentSlashes() {
            var files = _storageProvider.ListFiles("Subfolder1");
            Assert.That(files, Is.Not.Null);
            Assert.That(files.Count(), Is.EqualTo(2));
            var one = files.Single(x => x.GetName() == "one.txt");
            var two = files.Single(x => x.GetName() == "two.txt");

            Assert.That(one.GetPath(), Is.EqualTo("Subfolder1" + Path.DirectorySeparatorChar + "one.txt"));
            Assert.That(two.GetPath(), Is.EqualTo("Subfolder1" + Path.DirectorySeparatorChar + "two.txt"));
        }


        [Test]
        public void AnySlashInGetFileBecomesEnvironmentAppropriate() {
            var file1 = _storageProvider.GetFile(@"Subfolder1/one.txt");
            var file2 = _storageProvider.GetFile(@"Subfolder1\one.txt");
            Assert.That(file1.GetPath(), Is.EqualTo("Subfolder1" + Path.DirectorySeparatorChar + "one.txt"));
            Assert.That(file2.GetPath(), Is.EqualTo("Subfolder1" + Path.DirectorySeparatorChar + "one.txt"));
        }

        [Test]
        public void ListFoldersReturnsItemsWithShortPathAndEnvironmentSlashes() {
            var folders = _storageProvider.ListFolders(@"Subfolder1");
            Assert.That(folders, Is.Not.Null);
            Assert.That(folders.Count(), Is.EqualTo(1));
            Assert.That(folders.Single().GetName(), Is.EqualTo("SubSubfolder1"));
            Assert.That(folders.Single().GetPath(), Is.EqualTo(Path.Combine("Subfolder1", "SubSubfolder1")));
        }

        [Test]
        public void ParentFolderPathIsStillShort() {
            var subsubfolder = _storageProvider.ListFolders(@"Subfolder1").Single();
            var subfolder = subsubfolder.GetParent();
            Assert.That(subsubfolder.GetName(), Is.EqualTo("SubSubfolder1"));
            Assert.That(subsubfolder.GetPath(), Is.EqualTo(Path.Combine("Subfolder1", "SubSubfolder1")));
            Assert.That(subfolder.GetName(), Is.EqualTo("Subfolder1"));
            Assert.That(subfolder.GetPath(), Is.EqualTo("Subfolder1"));
        }

        [Test]
        public void CreateFolderAndDeleteFolderTakesAnySlash() {
            Assert.That(_storageProvider.ListFolders(@"Subfolder1").Count(), Is.EqualTo(1));
            _storageProvider.CreateFolder(@"SubFolder1/SubSubFolder2");
            _storageProvider.CreateFolder(@"SubFolder1\SubSubFolder3");
            Assert.That(_storageProvider.ListFolders(@"Subfolder1").Count(), Is.EqualTo(3));
            _storageProvider.DeleteFolder(@"SubFolder1/SubSubFolder2");
            _storageProvider.DeleteFolder(@"SubFolder1\SubSubFolder3");
            Assert.That(_storageProvider.ListFolders(@"Subfolder1").Count(), Is.EqualTo(1));
        }

        private IStorageFolder GetFolder(string path) {
            return _storageProvider.ListFolders(Path.GetDirectoryName(path))
                .SingleOrDefault(x => string.Equals(x.GetName(), Path.GetFileName(path), StringComparison.OrdinalIgnoreCase));
        }
        private IStorageFile GetFile(string path) {
            try {
                return _storageProvider.GetFile(path);
            }
            catch (ArgumentException) {
                return null;
            }
        }

        [Test]
        public void RenameFolderTakesShortPathWithAnyKindOfSlash() {
            Assert.That(GetFolder(@"SubFolder1/SubSubFolder1"), Is.Not.Null);
            _storageProvider.RenameFolder(@"SubFolder1\SubSubFolder1", @"SubFolder1/SubSubFolder2");
            _storageProvider.RenameFolder(@"SubFolder1\SubSubFolder2", @"SubFolder1\SubSubFolder3");
            _storageProvider.RenameFolder(@"SubFolder1/SubSubFolder3", @"SubFolder1\SubSubFolder4");
            _storageProvider.RenameFolder(@"SubFolder1/SubSubFolder4", @"SubFolder1/SubSubFolder5");
            Assert.That(GetFolder(Path.Combine("SubFolder1", "SubSubFolder1")), Is.Null);
            Assert.That(GetFolder(Path.Combine("SubFolder1", "SubSubFolder2")), Is.Null);
            Assert.That(GetFolder(Path.Combine("SubFolder1", "SubSubFolder3")), Is.Null);
            Assert.That(GetFolder(Path.Combine("SubFolder1", "SubSubFolder4")), Is.Null);
            Assert.That(GetFolder(Path.Combine("SubFolder1", "SubSubFolder5")), Is.Not.Null);
        }


        [Test]
        public void CreateFileAndDeleteFileTakesAnySlash() {
            Assert.That(_storageProvider.ListFiles(@"Subfolder1").Count(), Is.EqualTo(2));
            var alpha = _storageProvider.CreateFile(@"SubFolder1/alpha.txt");
            var beta = _storageProvider.CreateFile(@"SubFolder1\beta.txt");
            Assert.That(_storageProvider.ListFiles(@"Subfolder1").Count(), Is.EqualTo(4));
            Assert.That(alpha.GetPath(), Is.EqualTo(Path.Combine("SubFolder1", "alpha.txt")));
            Assert.That(beta.GetPath(), Is.EqualTo(Path.Combine("SubFolder1", "beta.txt")));
            _storageProvider.DeleteFile(@"SubFolder1\alpha.txt");
            _storageProvider.DeleteFile(@"SubFolder1/beta.txt");
            Assert.That(_storageProvider.ListFiles(@"Subfolder1").Count(), Is.EqualTo(2));
        }

        [Test]
        public void RenameFileTakesShortPathWithAnyKindOfSlash() {
            Assert.That(GetFile(@"Subfolder1/one.txt"), Is.Not.Null);
            _storageProvider.RenameFile(@"SubFolder1\one.txt", @"SubFolder1/testfile2.txt");
            _storageProvider.RenameFile(@"SubFolder1\testfile2.txt", @"SubFolder1\testfile3.txt");
            _storageProvider.RenameFile(@"SubFolder1/testfile3.txt", @"SubFolder1\testfile4.txt");
            _storageProvider.RenameFile(@"SubFolder1/testfile4.txt", @"SubFolder1/testfile5.txt");
            Assert.That(GetFile(Path.Combine("SubFolder1", "one.txt")), Is.Null);
            Assert.That(GetFile(Path.Combine("SubFolder1", "testfile2.txt")), Is.Null);
            Assert.That(GetFile(Path.Combine("SubFolder1", "testfile3.txt")), Is.Null);
            Assert.That(GetFile(Path.Combine("SubFolder1", "testfile4.txt")), Is.Null);
            Assert.That(GetFile(Path.Combine("SubFolder1", "testfile5.txt")), Is.Not.Null);
        }
    }
}



