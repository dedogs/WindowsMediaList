WindowsMediaList
================

I was working with a lot of media files (.mp4) located on my local drive. The media files were categorized within nth-deep sub folders. Watching the media, I was using Window Media Player, and I knew Windows Media Player can read a XML playlist, so, I wrote a small WPF application that created a Windows Play List for each folder.

Using recursion, I created a collection of media files, which I wrote out to an XML file on my local drive. After creating the XML file, I was able to double-click on the file and Windows Media Player would begin. The XML file was a Windows Media Play List.
