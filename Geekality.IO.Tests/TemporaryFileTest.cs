using System;
using System.IO;
using NUnit.Framework;

namespace Geekality.IO
{
    [TestFixture]
    class TemporaryFileTest
    {
        [Test]
        public void FileInfo_ReturnsWrappedFileInfo()
        {
            var file = new FileInfo(Path.GetTempFileName());
            using (var temporaryFile = new TemporaryFile(file))
            {
                Assert.AreSame(file, temporaryFile.FileInfo);
            }
        }

        [Test]
        public void ImplicitCastToFileInfo_ReturnsWrappedFileInfo()
        {
            using (var file = new TemporaryFile())
            {
                FileInfo fileInfo = file;
                Assert.AreSame(file.FileInfo, fileInfo);
            }
        }

        [Test]
        public void ImplicitCastToFileString_ReturnsFullPathOfTemporaryFile()
        {
            String expectedPath = Path.GetTempFileName();

            using (var temporaryFile = new TemporaryFile(expectedPath))
            {
                String path = temporaryFile;
                Assert.AreEqual(expectedPath, path);
            }
        }

        [Test]
        public void ExplicitCastFromFileInfo_CreatesTemporaryFile()
        {
            var file = new FileInfo(Path.GetTempFileName());
            Assume.That(file.Exists);

            using ((TemporaryFile) file)
            {
                file.Refresh();
                Assert.True(file.Exists);
            }

            file.Refresh();
            Assert.False(file.Exists);
        }

        [Test]
        public void ConstructorAndDispose_FileIsCreatedAndDeleted()
        {
            var file = new TemporaryFile();

            FileInfo fileInfo = file;
            Assert.True(fileInfo.Exists);

            file.Dispose();
            fileInfo.Refresh();
            Assert.False(fileInfo.Exists);
        }

        [Test]
        public void Finalizer_FileIsDeleted()
        {
            // Create file
            var file = new TemporaryFile();

            // Store fileinfo
            FileInfo fileInfo = file;
            Assume.That(fileInfo.Exists, "File should exist at this point.");

            // Drop reference and do GC
            file = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // File should be gone now
            fileInfo.Refresh();
            Assert.False(fileInfo.Exists, "File should not exist anymore");
        }
    }
}
