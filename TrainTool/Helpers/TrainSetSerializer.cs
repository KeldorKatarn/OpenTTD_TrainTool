// <copyright file="TrainSetSerializer.cs" company="VacuumBreather">
//      Copyright © 2014 VacuumBreather. All rights reserved.
// </copyright>
// <license type="X11/MIT">
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
//      The above copyright notice and this permission notice shall be included in
//      all copies or substantial portions of the Software.
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//      THE SOFTWARE.
// </license>

namespace TrainTool.Helpers
{
    #region Using Directives

    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Model;

    #endregion

    /// <summary>
    ///     Helper class to serialize and deserialize a train set.
    /// </summary>
    public static class TrainSetSerializer
    {
        #region Class Methods

        /// <summary>
        ///     Loads a trainSet asynchronously from a stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        /// <returns>
        ///     The loaded train set.
        /// </returns>
        /// <exception cref="ArgumentNullException">When <paramref name="stream" /> is null.</exception>
        public static async Task<TrainSet> LoadFromAsync(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);

            return await Task.Factory.StartNew(
                () =>
                {
                    var dataContractSerializer = new DataContractSerializer(typeof(TrainSet));

                    TrainSet trainSet;

                    using (
                        XmlDictionaryReader reader =
                            XmlDictionaryReader.CreateDictionaryReader(new XmlTextReader(stream)))
                    {
                        trainSet = (TrainSet)dataContractSerializer.ReadObject(reader, false);
                    }

                    return trainSet;
                });
        }

        /// <summary>
        ///     Saves a trainSet asynchronously to a stream.
        /// </summary>
        /// <param name="trainSet">The trainSet to save.</param>
        /// <param name="stream">The stream to save to.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="trainSet" /> or <paramref name="stream" />is null.</exception>
        public static async Task SaveToAsync(TrainSet trainSet, Stream stream)
        {
            Contract.Requires<ArgumentNullException>(trainSet != null);
            Contract.Requires<ArgumentNullException>(stream != null);

            var serializer = new DataContractSerializer(typeof(TrainSet));
            var xmlWriterSettings = new XmlWriterSettings
                                    {
                                        CheckCharacters = true,
                                        Encoding = new UTF8Encoding(),
                                        Indent = true
                                    };

            await Task.Factory.StartNew(
                () =>
                {
                    using (
                        XmlDictionaryWriter writer =
                            XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(stream, xmlWriterSettings)))
                    {
                        serializer.WriteObject(writer, trainSet);
                    }
                });
        }

        #endregion
    }
}