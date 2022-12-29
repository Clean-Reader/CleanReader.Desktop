// Copyright (c) Richasy. All rights reserved.

using System.Text;

namespace CleanReader.Services.Epub;

/// <summary>
/// 编码辅助类.
/// </summary>
public class EncodingHelper
{
    private readonly Ude.CharsetDetector _ude = new();
    private readonly List<string> _singleEncodings = new();
    private bool _started = false;

    /// <summary>
    /// If the detection has reached a decision.
    /// </summary>
    /// <value>The done.</value>
    public bool Done { get; set; }

    /// <summary>
    /// Detected encoding name.
    /// </summary>
    public string EncodingName { get; set; }

    /// <summary>
    /// If the data contains textual data.
    /// </summary>
    public bool IsText { get; set; }

    /// <summary>
    /// If the file or data has any mark indicating encoding information (byte order mark).
    /// </summary>
    public bool HasByteOrderMark { get; set; }

    /// <summary>
    /// Tries to detect the file encoding.
    /// </summary>
    /// <param name="inputStream">The input stream.</param>
    /// <param name="defaultIfNotDetected">The default encoding if none was detected.</param>
    /// <returns><see cref="Encoding"/>.</returns>
    public static Encoding DetectFileEncoding(Stream inputStream, Encoding defaultIfNotDetected = null)
    {
        var det = new EncodingHelper();
        det.Detect(inputStream);
        return det.Complete() ?? defaultIfNotDetected;
    }

    /// <summary>
    /// Tries to detect the file encoding.
    /// </summary>
    /// <param name="inputData">The input data.</param>
    /// <param name="start">The start.</param>
    /// <param name="count">The count.</param>
    /// <param name="defaultIfNotDetected">The default encoding if none was detected.</param>
    /// <returns><see cref="Encoding"/>.</returns>
    public static Encoding DetectFileEncoding(byte[] inputData, int start, int count, Encoding defaultIfNotDetected = null)
    {
        var det = new EncodingHelper();
        det.Detect(inputData, start, count);
        return det.Complete() ?? defaultIfNotDetected;
    }

    /// <summary>
    /// Detects if contains textual data.
    /// </summary>
    /// <param name="rawData">The raw data.</param>
    /// <returns>If contains textual data.</returns>
    public static bool CheckForTextualData(byte[] rawData) => CheckForTextualData(rawData, 0, rawData.Length);

    /// <summary>
    /// Detects if contains textual data.
    /// </summary>
    /// <param name="rawData">The raw data.</param>
    /// <param name="start">The start.</param>
    /// <param name="count">The count.</param>
    /// <returns>If contains textual data.</returns>
    public static bool CheckForTextualData(byte[] rawData, int start, int count)
    {
        if (rawData.Length < count || count < 4 || start + 1 >= count)
        {
            return true;
        }

        if (CheckForByteOrderMark(rawData, start))
        {
            return true;
        }

        // http://stackoverflow.com/questions/910873/how-can-i-determine-if-a-file-is-binary-or-text-in-c
        // http://www.gnu.org/software/diffutils/manual/html_node/Binary.html
        // count the number od null bytes sequences
        // considering only sequeces of 2 0s: "\0\0" or control characters below 10
        var nullSequences = 0;
        var controlSequences = 0;
        for (var i = start + 1; i < count; i++)
        {
            if (rawData[i - 1] == 0 && rawData[i] == 0)
            {
                if (++nullSequences > 1)
                {
                    break;
                }
            }
            else if (rawData[i - 1] == 0 && rawData[i] < 10)
            {
                ++controlSequences;
            }
        }

        // is text if there is no null byte sequences or less than 10% of the buffer has control caracteres
        return nullSequences == 0 && (controlSequences <= (rawData.Length / 10));
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public void Reset()
    {
        _started = false;
        Done = false;
        HasByteOrderMark = false;
        _singleEncodings.Clear();
        _ude.Reset();
        EncodingName = null;
    }

    /// <summary>
    /// Detects the encoding of textual data of the specified input data.
    /// </summary>
    /// <param name="inputData">The input data.</param>
    /// <returns>Detected encoding name.</returns>
    public string Detect(Stream inputData)
    {
        const int bufferSize = 16 * 1024;
        const int maxIterations = 20 * 1024 * 1024 / bufferSize;
        var i = 0;
        var buffer = new byte[bufferSize];
        while (i++ < maxIterations)
        {
            var sz = inputData.Read(buffer, 0, (int)buffer.Length);
            if (sz <= 0)
            {
                break;
            }

            Detect(buffer, 0, sz);
            if (Done)
            {
                break;
            }
        }

        Complete();
        return EncodingName;
    }

    /// <summary>
    /// Detects the encoding of textual data of the specified input data.
    /// </summary>
    /// <param name="inputData">The input data.</param>
    /// <param name="start">The start.</param>
    /// <param name="count">The count.</param>
    /// <returns>Detected encoding name.</returns>
    public string Detect(byte[] inputData, int start, int count)
    {
        if (Done)
        {
            return EncodingName;
        }

        if (!_started)
        {
            Reset();
            _started = true;
            if (!CheckForTextualData(inputData, start, count))
            {
                IsText = false;
                Done = true;
                return EncodingName;
            }

            HasByteOrderMark = CheckForByteOrderMark(inputData, start);
            IsText = true;
        }

        // execute charset detector.
        _ude.Feed(inputData, start, count);
        _ude.DataEnd();
        if (_ude.IsDone() && !string.IsNullOrEmpty(_ude.Charset))
        {
            Done = true;
            return EncodingName;
        }

        const int bufferSize = 4 * 1024;

        // singular buffer detection
        if (_singleEncodings.Count < 2000)
        {
            var u = new Ude.CharsetDetector();
            var step = (count - start) < bufferSize ? (count - start) : bufferSize;
            for (var i = start; i < count; i += step)
            {
                u.Reset();
                if (i + step > count)
                {
                    u.Feed(inputData, i, count - i);
                }
                else
                {
                    u.Feed(inputData, i, step);
                }

                u.DataEnd();
                if (u.Confidence > 0.3 && !string.IsNullOrEmpty(u.Charset))
                {
                    _singleEncodings.Add(u.Charset);
                }
            }
        }

        return EncodingName;
    }

    /// <summary>
    /// Finalize detection phase and gets detected encoding name.
    /// </summary>
    /// <returns><see cref="Encoding"/>.</returns>
    public Encoding Complete()
    {
        Done = true;
        _ude.DataEnd();
        if (_ude.IsDone() && !string.IsNullOrEmpty(_ude.Charset))
        {
            EncodingName = _ude.Charset;
        }
        else if (_singleEncodings.Count > 0)
        {
            // vote for best encoding
            EncodingName = _singleEncodings.GroupBy(i => i)
                .OrderByDescending(i => i.Count() *
               (i.Key.StartsWith("UTF-32") ? 2 :
               i.Key.StartsWith("UTF-16") ? 1.8 :
               i.Key.StartsWith("UTF-8") ? 1.5 :
               i.Key.StartsWith("UTF-7") ? 1.3 :
               i.Key != "ASCII" ? 1 : 0.2))
                .Select(i => i.Key).FirstOrDefault();
        }

        return !string.IsNullOrEmpty(EncodingName) ? Encoding.GetEncoding(EncodingName) : null;
    }

    /// <summary>
    /// Detects if data has bytes order mark to indicate its encoding for textual data.
    /// </summary>
    /// <param name="rawData">The raw data.</param>
    /// <param name="start">The start.</param>
    /// <returns>if data has bytes order mark to indicate its encoding for textual data.</returns>
    private static bool CheckForByteOrderMark(byte[] rawData, int start = 0)
    {
        if (rawData.Length - start < 4)
        {
            return false;
        }

        // Detect encoding correctly (from Rick Strahl's blog)
        // http://www.west-wind.com/weblog/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
        if (rawData[start] == 0xef && rawData[start + 1] == 0xbb && rawData[start + 2] == 0xbf)
        {
            // Encoding.UTF8;
            return true;
        }
        else if (rawData[start] == 0xfe && rawData[start + 1] == 0xff)
        {
            // Encoding.Unicode;
            return true;
        }
        else if (rawData[start] == 0 && rawData[start + 1] == 0 && rawData[start + 2] == 0xfe && rawData[start + 3] == 0xff)
        {
            // Encoding.UTF32;
            return true;
        }
        else if (rawData[start] == 0x2b && rawData[start + 1] == 0x2f && rawData[start + 2] == 0x76)
        {
            // Encoding.UTF7;
            return true;
        }

        return false;
    }
}
