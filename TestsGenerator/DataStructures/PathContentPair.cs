using System;

namespace TestsGenerator.DataStructures
{
    public class PathContentPair
    {
        public string Path
        { get; protected set; }

        public string Content
        { get; protected set; }

        public PathContentPair(string path, string content)
        {
            if ((path == null) || (content == null))
            {
                throw new ArgumentException("Arguments shouldn't be null");
            }
            Path = path;
            Content = content;
        }
    }
}
