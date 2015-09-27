Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Index
Imports Lucene.Net.Search

Public Class Search

    Public Shared Function Index(lst As List(Of String)) As Lucene.Net.Store.RAMDirectory
        Dim idxDir As New Lucene.Net.Store.RAMDirectory
        Dim Analyzer As New StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29)
        Dim idxWriter As New IndexWriter(idxDir, Analyzer, True, IndexWriter.MaxFieldLength.UNLIMITED)
        Dim doc As New Lucene.Net.Documents.Document
        Dim fld As Lucene.Net.Documents.Field

        For Each item In lst
            doc = New Lucene.Net.Documents.Document()
            If item IsNot Nothing Then
                fld = New Lucene.Net.Documents.Field("id", lst.IndexOf(item), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.NO)
                doc.Add(fld)
                fld = New Lucene.Net.Documents.Field("field", CStr(item), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED, Lucene.Net.Documents.Field.TermVector.NO)
                doc.Add(fld)
                idxWriter.AddDocument(doc)
            End If
        Next
        idxWriter.Optimize()
        idxWriter.Dispose()

        Return idxDir
    End Function

    Public Shared Function Search(idxDir As Lucene.Net.Store.RAMDirectory, txt As String, NumberOfResults As Integer) As List(Of Integer)
        Dim Analyzer As New StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29)
        Dim idxSearcher As Lucene.Net.Search.Searcher = New Lucene.Net.Search.IndexSearcher(idxDir)
        Dim QryParser As Lucene.Net.QueryParsers.QueryParser
        Dim Qry As Lucene.Net.Search.Query
        Dim Results As ScoreDoc()

        Dim dResults As New Dictionary(Of String, List(Of Integer))
        Dim lstResults As List(Of Integer)

        Dim fld As String = "field"
        lstResults = New List(Of Integer)
        QryParser = New Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_29, fld, Analyzer)
        Qry = QryParser.Parse(txt)

        Results = idxSearcher.Search(Qry, NumberOfResults).ScoreDocs

        If Results.Count <> 0 Then
            lstResults = New List(Of Integer)
            For Each hit In Results
                If lstResults.Contains(idxSearcher.Doc(hit.Doc).Get("id")) = False Then
                    lstResults.Add(idxSearcher.Doc(hit.Doc).Get("id"))
                End If
            Next

            dResults.Add(fld, lstResults)
        End If

        Return lstResults
    End Function
End Class
