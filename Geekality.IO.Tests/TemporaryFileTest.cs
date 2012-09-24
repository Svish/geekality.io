using System;
using System.IO;
using NUnit.Framework;

namespace Geekality.IO
{
    [TestFixture]
    class TemporaryFileTest
    {
        [Test]
        public void Construct_Default_EmptyFileIsCreated()
        {
            using (var file = new TemporaryFile())
            {
                Assert.True(file.FileInfo.Exists, "File was not created.");
                Assert.AreEqual(0, file.FileInfo.Length, "File was not empty.");
            }
        }

        [Test]
        public void Constructor_FileInfo_UsesGivenFileInfo()
        {
            var fileInfo = new FileInfo(Path.GetTempFileName());

            using (var temporaryFile = new TemporaryFile(fileInfo))
                Assert.AreSame(fileInfo, temporaryFile.FileInfo);
        }

        [Test]
        public void Constructor_Stream_FileIsInitializedWithStreamContents()
        {
            using (var s = EmbeddedResource.Get<TemporaryFileTest>("EmbeddedResourceTest.txt"))
            using (var file = new TemporaryFile(s))
                Assert.AreEqual(EmbeddedResourceTest.CONTENTS, File.ReadAllText(file));
        }

        [Test]
        public void Constructor_Stream_StreamIsClosed()
        {
            using (var s = EmbeddedResource.Get<TemporaryFileTest>("EmbeddedResourceTest.txt"))
            using (var file = new TemporaryFile(s))
                Assert.False(s.CanRead, "Stream should be closed.");
        }

        [Test]
        public void Dispose_CreatedFile_FileIsDeleted()
        {
            var file = new TemporaryFile();

            FileInfo fileInfo = file;
            Assume.That(fileInfo.Exists, "File should've been created.");

            file.Dispose();
            fileInfo.Refresh();
            Assert.False(fileInfo.Exists, "File was not deleted.");
        }

        [Test]
        public void Dispose_NonExistingFile_DoesNotCrash()
        {
            using (var file = new TemporaryFile())
                file.FileInfo.Delete();
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

            using ((TemporaryFile)file)
            {
                file.Refresh();
                Assert.True(file.Exists);
            }

            file.Refresh();
            Assert.False(file.Exists);
        }
    }
}
