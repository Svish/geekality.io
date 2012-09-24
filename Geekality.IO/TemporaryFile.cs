using System;
using System.IO;

namespace Geekality.IO
{
    /// <summary>
    /// A simple wrapper which deletes a file on dispose (or in finalizer).
    /// </summary>
    public sealed class TemporaryFile : IDisposable
    {
        private volatile bool disposed;
        private readonly FileInfo file;

        /// <summary>
        /// The wrapped file.
        /// </summary>
        public FileInfo FileInfo { get { return file; } }

        /// <summary>
        /// Creates and wraps a new temporary file using <see cref="Path.GetTempFileName"/>.
        /// </summary>
        public TemporaryFile() : this(Path.GetTempFileName()) { }

        /// <summary>
        /// Wraps an existing file as a temporary file.
        /// </summary>
        /// <param name="path">Name of file to wrap.</param>
        public TemporaryFile(string fileName) : this(new FileInfo(fileName)) { }

        /// <summary>
        /// Wraps an existing file as a temporary file.
        /// </summary>
        /// <param name="file">File to wrap.</param>
        public TemporaryFile(FileInfo temporaryFile)
        {
            file = temporaryFile;
        }

        /// <summary>
        /// Creates and wraps a new temporary file with the given stream copied to it.
        /// </summary>
        /// <param name="initialFileContents"></param>
        public TemporaryFile(Stream initialFileContents) : this()
        {
            using (var file = new FileStream(this, FileMode.Open))
                initialFileContents.CopyTo(file);
        }

        /// <summary>
        /// Implicit conversion to <see cref="FileInfo"/> for easy use as method parameter, etc.
        /// <example>
        /// Example:
        /// <code>using(var file = new TemporaryFile())
        /// {
        ///     var array = new FileInfo[] { file };
        /// }</code></example>
        /// <remarks>Remember to keep a reference to the <see cref="TemporaryFile"/>, otherwise it might get finalized and disposed before you planned to :)</remarks>
        /// </summary>
        public static implicit operator FileInfo(TemporaryFile temporaryFile)
        {
            return temporaryFile.file;
        }

        /// <summary>
        /// Implicit conversion to <see cref="string"/> for easy use as method parameter, etc.
        /// <example>
        /// Example:
        /// <code>using(var file = new TemporaryFile())
        /// {
        ///     
        /// }</code></example>
        /// <remarks>Remember to keep a reference to the <see cref="TemporaryFile"/>, otherwise it might get finalized and disposed before you planned to :)</remarks>
        /// </summary>
        public static implicit operator string(TemporaryFile temporaryFile)
        {
            return temporaryFile.file.FullName;
        }

        /// <summary>
        /// Explicit conversion from <see cref="FileInfo"/> for easy wrapping.
        /// <example>
        /// Example:
        /// <code>var file = new FileInfo("tmp.txt");
        /// using((TemporaryFile) file)
        /// {
        ///     // Do stuff with file
        /// }</code>
        /// </example>
        /// </summary>
        public static explicit operator TemporaryFile(FileInfo temporaryFile)
        {
            return new TemporaryFile(temporaryFile);
        }

        /// <summary>
        /// Silently (exceptions swallowed) try to delete the temporary file.
        /// </summary>
        public void Dispose()
        {
            try
            {
                file.Delete();
                disposed = true;
            }
            catch (Exception) { } // Ignore
        }


        /// <summary>
        /// Call <see cref="Dispose"/> if nobody else has.
        /// </summary>
        ~TemporaryFile()
        {
            if (!disposed)
                Dispose();
        }
    }
}
