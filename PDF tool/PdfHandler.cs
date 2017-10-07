using System.Collections.Generic;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PDF_tool {
    public class PdfHandler {
        public List<PdfDocument> Input { get; } = new List<PdfDocument>();
        private readonly PdfDocument _output = new PdfDocument();

        /// <summary>
        /// Add the given pages from the given document to the output document.
        /// </summary>
        /// <param name="pdf">The input pdf.</param>
        /// <param name="pages">The pages to take from the input pdf.</param>
        public void AddPagesToDoc(PdfDocument pdf, int[] pages) {
            foreach (var page in pages) {
                _output.AddPage(pdf.Pages[page]);
            }
        }

        /// <summary>
        /// Load pdf files as input documents.
        /// </summary>
        /// <param name="paths">List of paths of the input documents.</param>
        /// <returns>Returns true if at least one pdf document was successfully loaded.</returns>
        public async Task<bool> LoadAsync(string[] paths) {
            var input = new List<PdfDocument>();
            foreach (var path in paths) {
                if (PdfReader.TestPdfFile(path) > 0) {
                    await Task.Run(() => input.Add(PdfReader.Open(path, PdfDocumentOpenMode.Import)));
                }
            }
            Input.AddRange(input);
            return input.Count > 0;
        }

        /// <summary>
        /// Save the output pdf to disk.
        /// </summary>
        /// <param name="path">Path to save the pdf to.</param>
        /// <returns>Returns true if the pdf document was successfully saved.</returns>
        public async Task<bool> SaveAsync(string path) {
            await Task.Run(() => _output.Save(path));
            return PdfReader.TestPdfFile(path) > 0;
        }
    }
}