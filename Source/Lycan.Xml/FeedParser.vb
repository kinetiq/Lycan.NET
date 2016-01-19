Imports Ether.Outcomes
Imports Lycan.Core
Imports System.Xml

Public Class FeedParser
    public Function Parse(doc As XDocument) As FeedModel
        Dim Feed = New FeedModel

        'BGG API2: https://boardgamegeek.com/wiki/page/BGG_XML_API2
        
        Feed.Subject = doc.<thread>.<subject>.Value()
        Feed.ArticleCount = doc.<thread>.@numarticles
        
        For Each Item In doc.<thread>.<articles>...<article>
            Dim Post = new ArticleModel

            Post.ArticleID = CInt(Item.@id)
            Post.Subject = Item.<subject>.Value()
            Post.Body = Item.<body>.Value()
            Post.Link = Item.@link
            Post.PostDate = Item.@postdate
            Post.User = Item.@username
            Post.Edits = CInt(Item.@numedits)

            Feed.Articles.Add(Post)
        Next

        return Feed
    End Function
End Class
