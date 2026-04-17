Attribute VB_Name = "MorningReportSaver"
' ================================================================
' Outlook VBA Macro - Auto-save Morning Report Excel attachments
' ================================================================
'
' SETUP (one time only):
'
'   STEP 1 - Import this macro:
'     In Outlook press Alt+F11
'     File -> Import File -> select this .bas file
'     Close the VBA editor
'
'   STEP 2 - Create the Outlook rule:
'     Home -> Rules -> Manage Rules & Alerts -> New Rule
'     -> Start from a blank rule: "Apply rule on messages I receive"
'     -> Condition: "with specific words in the subject"
'          -> type: Morning Report -> Add -> OK
'     -> Action: "run a script"
'          -> select:  MorningReportSaver.SaveMorningReportAttachment
'     -> Finish (give it any name, e.g. "Save Morning Report")
'
' ================================================================

Const SAVE_FOLDER As String = "C:\Users\H259507\OneDrive - Halliburton\Documents\MorningReports\incoming\"

' ── Called by Outlook rule (Item = the incoming email) ──────────────────────
Public Sub SaveMorningReportAttachment(Item As Outlook.MailItem)
    On Error GoTo ErrHandler

    EnsureFolder

    Dim att As Outlook.Attachment
    Dim saved As Integer
    saved = 0

    For Each att In Item.Attachments
        Dim ext As String
        ext = LCase(Right(att.FileName, 5))
        If ext = ".xlsx" Or LCase(Right(att.FileName, 4)) = ".xls" Then
            Dim saveName As String
            saveName = ExtractReportDateStr(att.FileName) & "_" & Format(Now, "HH-nn") & "_" & att.FileName
            att.SaveAsFile SAVE_FOLDER & saveName
            saved = saved + 1
            ' After saving, prune older files for the same rig — keep only the 2 newest
            PruneRigFiles ExtractRigName(att.FileName)
        End If
    Next att

    Exit Sub

ErrHandler:
    MsgBox "SaveMorningReportAttachment ERROR:" & vbCrLf & _
           "Error " & Err.Number & ": " & Err.Description & vbCrLf & vbCrLf & _
           "Target folder: " & SAVE_FOLDER, vbCritical, "Morning Report Macro"
End Sub

' ── Extract the report date from the original filename as YYYY-MM-DD ─────────
' e.g. "Enabler MWD Morning Report 5.04.2026.xls" -> "2026-04-05"
' Falls back to today if no recognisable date is found.
Private Function ExtractReportDateStr(fileName As String) As String
    Dim base As String
    base = fileName
    ' Strip extension
    Dim dotPos As Long
    dotPos = InStrRev(base, ".")
    If dotPos > 0 Then base = Left(base, dotPos - 1)
    ' Find the last space — the date token comes after it
    Dim spacePos As Long
    spacePos = InStrRev(base, " ")
    If spacePos > 0 Then
        Dim datePart As String
        datePart = Trim(Mid(base, spacePos + 1))
        If datePart Like "##.##.####" Or datePart Like "#.##.####" Or _
           datePart Like "##.#.####"  Or datePart Like "#.#.####" Then
            Dim parts() As String
            parts = Split(datePart, ".")
            If UBound(parts) = 2 Then
                Dim dd As Integer, mm As Integer, yyyy As Integer
                On Error Resume Next
                dd   = CInt(parts(0))
                mm   = CInt(parts(1))
                yyyy = CInt(parts(2))
                If Err.Number = 0 And yyyy > 2000 And mm >= 1 And mm <= 12 _
                                  And dd >= 1 And dd <= 31 Then
                    ExtractReportDateStr = Format(DateSerial(yyyy, mm, dd), "YYYY-MM-DD")
                    On Error GoTo 0
                    Exit Function
                End If
                On Error GoTo 0
            End If
        End If
    End If
    ' Fallback: use today's date
    ExtractReportDateStr = Format(Now, "YYYY-MM-DD")
End Function

' ── Extract rig name from original attachment filename ───────────────────────
' Handles both:
'   "Enabler MWD Morning Report 23.03.2026.xls"  -> "Enabler MWD"
'   "Morning Report COSLPromoter 23.03.2026.xlsx" -> "COSLPromoter"
Private Function ExtractRigName(fileName As String) As String
    ' Strip extension and trailing date (e.g. " 23.03.2026")
    Dim base As String
    base = fileName
    Dim dotPos As Long
    dotPos = InStrRev(base, ".")
    If dotPos > 0 Then base = Left(base, dotPos - 1)
    ' Remove trailing date pattern DD.MM.YYYY (variable length: 9-11 chars incl. space)
    Dim dateLen As Long
    If base Like "* ##.##.####" Then
        dateLen = 11          ' " DD.MM.YYYY"
    ElseIf base Like "* #.##.####" Or base Like "* ##.#.####" Then
        dateLen = 10          ' " D.MM.YYYY" or " DD.M.YYYY"
    ElseIf base Like "* #.#.####" Then
        dateLen = 9           ' " D.M.YYYY"
    Else
        dateLen = 0
    End If
    If dateLen > 0 Then
        base = Left(base, Len(base) - dateLen)
        base = RTrim(base)
    End If

    Dim lower As String
    lower = LCase(base)
    Dim idx As Long

    ' Case 1: rig name comes BEFORE "morning report"
    idx = InStr(lower, " morning report")
    If idx > 1 Then
        ExtractRigName = Trim(Left(base, idx - 1))
        Exit Function
    End If

    ' Case 2: rig name comes AFTER "morning report" (reversed format)
    idx = InStr(lower, "morning report ")
    If idx > 0 Then
        ExtractRigName = Trim(Mid(base, idx + Len("morning report ")))
        Exit Function
    End If

    ExtractRigName = ""
End Function

' ── Keep only the 2 newest files for a given rig in the incoming folder ──────
' Files are named YYYY-MM-DD_HH-MM_<RigName> Morning Report ...
' Sorting alphabetically = chronological because of the leading timestamp.
Private Sub PruneRigFiles(rigName As String)
    If rigName = "" Then Exit Sub

    Dim lowerRig As String
    lowerRig = LCase(rigName)

    ' Collect matching filenames into a simple array (VBA has no List)
    Dim files(200) As String
    Dim count As Integer
    count = 0

    ' Saved files are prefixed: YYYY-MM-DD_HH-MM_<RigName> Morning Report ...
    ' Match by rig name only (the timestamp prefix means rig name is always present)
    Dim f As String
    f = Dir(SAVE_FOLDER & "*.xls*")
    Do While f <> "" And count < 200
        Dim lowerF As String
        lowerF = LCase(f)
        If InStr(lowerF, lowerRig) > 0 And InStr(lowerF, "morning report") > 0 Then
            files(count) = f
            count = count + 1
        End If
        f = Dir()
    Loop

    If count <= 2 Then Exit Sub   ' nothing to prune

    ' Bubble-sort ascending (oldest first) — filenames sort chronologically
    Dim i As Integer, j As Integer, tmp As String
    For i = 0 To count - 2
        For j = 0 To count - i - 2
            If files(j) > files(j + 1) Then
                tmp = files(j)
                files(j) = files(j + 1)
                files(j + 1) = tmp
            End If
        Next j
    Next i

    ' Delete everything except the 2 newest (last 2 in sorted order)
    For i = 0 To count - 3
        On Error Resume Next
        Kill SAVE_FOLDER & files(i)
        On Error GoTo 0
    Next i
End Sub

' ── Manual test: run this directly from the VBA editor (F5) ─────────────────
' It grabs the currently selected email in Outlook and tries to save attachments.
Public Sub TestWithSelectedEmail()
    On Error GoTo ErrHandler

    EnsureFolder

    Dim sel As Outlook.Selection
    Set sel = Application.ActiveExplorer.Selection

    If sel.Count = 0 Then
        MsgBox "No email selected. Select a Morning Report email in Outlook first.", _
               vbExclamation, "Test"
        Exit Sub
    End If

    Dim Item As Outlook.MailItem
    Set Item = sel.Item(1)

    Dim att As Outlook.Attachment
    Dim saved As Integer
    saved = 0
    Dim log As String
    log = "Email: " & Item.Subject & vbCrLf & _
          "Attachments: " & Item.Attachments.Count & vbCrLf & vbCrLf

    For Each att In Item.Attachments
        log = log & "  - " & att.FileName
        Dim ext As String
        ext = LCase(Right(att.FileName, 5))
        If ext = ".xlsx" Or LCase(Right(att.FileName, 4)) = ".xls" Then
            Dim saveName As String
            saveName = ExtractReportDateStr(att.FileName) & "_" & Format(Now, "HH-nn") & "_" & att.FileName
            att.SaveAsFile SAVE_FOLDER & saveName
            saved = saved + 1
            PruneRigFiles ExtractRigName(att.FileName)
            log = log & " -> SAVED"
        Else
            log = log & " (skipped, not Excel)"
        End If
        log = log & vbCrLf
    Next att

    log = log & vbCrLf & "Saved " & saved & " file(s) to:" & vbCrLf & SAVE_FOLDER

    MsgBox log, vbInformation, "Morning Report Test Result"
    Exit Sub

ErrHandler:
    MsgBox "TEST ERROR:" & vbCrLf & _
           "Error " & Err.Number & ": " & Err.Description & vbCrLf & vbCrLf & _
           "Target folder: " & SAVE_FOLDER, vbCritical, "Morning Report Test"
End Sub

' ── Helper ───────────────────────────────────────────────────────────────────
Private Sub EnsureFolder()
    If Dir(SAVE_FOLDER, vbDirectory) = "" Then
        MkDir "C:\Users\H259507\OneDrive - Halliburton\Documents\MorningReports"
        MkDir SAVE_FOLDER
    End If
End Sub
