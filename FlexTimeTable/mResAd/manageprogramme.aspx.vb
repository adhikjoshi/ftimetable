﻿Public Class manageprogramme
    Inherits System.Web.UI.Page
    Private TabHeader() As String = {"Core Subjects", "Service Subjects", "Site Clusters"}

#Region "General"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            getDepartment1.loadFaculty(User.Identity.Name)
            loadAllClusters()
            TabContainer1.ActiveTabIndex = 0
            setTabHeader(0)
            logSave.Text = "Save"
        End If
    End Sub

    Private Sub manageprogramme_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        getDepartment1.SetLabel(False, 220)
    End Sub


    Sub loadQualification(ByVal DepartID As Integer)
        cboQualification.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        cboQualification.DataSource = (From p In vContext.qualifications Where p.DepartmentID = DepartID Select p.longName, p.ID)
        cboQualification.DataTextField = "longName"
        cboQualification.DataValueField = "ID"
        cboQualification.DataBind()
        displayQualificationDetails()
    End Sub

    Sub setcontrol(ByVal vEnabled As Boolean)
        btnClusterAdd.Enabled = vEnabled
        btnClusterRemove.Enabled = vEnabled
        btnCoreAdd.Enabled = vEnabled
        btnServiceAdd.Enabled = vEnabled
        btnCoreRemove.Enabled = vEnabled
        btnServiceRemove.Enabled = vEnabled
    End Sub

    Sub displayQualificationDetails()
        lstCoreSubjects.Items.Clear()
        lstServiceSubjects.Items.Clear()
        lstSelectedCoreSubjects.Items.Clear()
        lstSelectedServiceSubject.Items.Clear()
        loadCoreSubjects("", getDepartment1.getID)
        loadServiceSubjects("")
        If cboQualification.Items.Count > 0 Then
            LoadQualificationSubjects()
            LoadQualificationClusters()
            setcontrol(True)
        Else
            setcontrol(False)
        End If

    End Sub

    Sub LoadQualificationSubjects()
        lstSelectedCoreSubjects.Items.Clear()
        lstSelectedServiceSubject.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim programmesubjectList = (From p In vContext.programmesubjects
                                      Order By p.subject.longName
                                        Where p.QualID = CInt(cboQualification.SelectedValue) And
                                              p.Level = CInt(cboLevel.SelectedValue)
                                               Select p).ToList
        For Each prog As programmesubject In programmesubjectList
            Dim oldstr = getOldStr(prog.SubjectID)
            Dim vItem As New ListItem(prog.subject.longName + "[" + prog.subject.Code + "]" + oldstr, CStr(prog.SubjectID))
            If prog.subject.DepartmentID = getDepartment1.getID Then
                'core subject
                lstSelectedCoreSubjects.Items.Add(vItem)
                lstSelectedCoreSubjects.SelectedIndex = -1
            Else
                'service subject
                lstSelectedServiceSubject.Items.Add(vItem)
                lstSelectedServiceSubject.SelectedIndex = -1
            End If
        Next
    End Sub

    Sub LoadQualificationClusters()
        lstSelectedClusters.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim QualClusters = (From p In vContext.qualifications
                            Where p.ID = CInt(cboQualification.SelectedValue)
                               Select p.siteclusters).FirstOrDefault
        For Each prog As sitecluster In QualClusters
            With prog
                Dim vItem As New ListItem(.longName, CStr(.ID))
                lstSelectedClusters.Items.Add(vItem)
                lstSelectedClusters.SelectedIndex = -1
            End With
        Next
    End Sub

    Sub setTabHeader(ByVal index As Integer)
        TabContainer1.ActiveTabIndex = index
        phLevel.Visible = CBool(IIf(index = 2, False, True))
    End Sub

    Private Sub cboQualification_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboQualification.SelectedIndexChanged
        displayQualificationDetails()
    End Sub

    Protected Sub cboLevel_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboLevel.SelectedIndexChanged
        displayQualificationDetails()
    End Sub

    Protected Sub btnCoreSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCoreSearch.Click
        loadCoreSubjects(txtCoreSearch.Text, getDepartment1.getID)
    End Sub

    Private Sub btnServiceSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnServiceSearch.Click
        loadServiceSubjects(txtServiceSearch.Text)
    End Sub
#End Region

#Region "Subjects"

    Protected Sub SaveSubjects()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vProgrammeSubject = (From p In vContext.programmesubjects Where p.QualID = CInt(cboQualification.SelectedValue) Select p)
        'delete subjects
        For Each prog As programmesubject In vProgrammeSubject
            'if already exists in both core and service then leave. if not delete
            Dim SubjectExists = False
            For Each vSubject As ListItem In lstSelectedCoreSubjects.Items
                If prog.SubjectID = CInt(vSubject.Value) Then
                    SubjectExists = True
                    Exit For
                End If
            Next
            For Each vSubject As ListItem In lstSelectedServiceSubject.Items
                If prog.SubjectID = CInt(vSubject.Value) Then
                    SubjectExists = True
                    Exit For
                End If
            Next
            If Not SubjectExists Then
                vContext.DeleteObject(prog)
            End If
        Next
        'add core  subjects
        For Each vSubject As ListItem In lstSelectedCoreSubjects.Items
            Dim SubjectExists = False
            For Each prog As programmesubject In vProgrammeSubject
                If prog.SubjectID = CInt(vSubject.Value) Then
                    SubjectExists = True
                    Exit For
                End If
            Next
            If Not SubjectExists Then
                Dim ProgSubject As New programmesubject With {
                                      .QualID = CInt(cboQualification.SelectedValue),
                                      .SubjectID = CInt(vSubject.Value),
                                      .Level = CInt(cboLevel.SelectedValue)}
                vContext.programmesubjects.AddObject(ProgSubject)
            End If
        Next
        'add service subjects
        For Each vSubject As ListItem In lstSelectedServiceSubject.Items
            Dim SubjectExists = False
            For Each prog As programmesubject In vProgrammeSubject
                If prog.SubjectID = CInt(vSubject.Value) Then
                    SubjectExists = True
                    Exit For
                End If
            Next
            If Not SubjectExists Then
                Dim ProgSubject As New programmesubject With {
                                      .QualID = CInt(cboQualification.SelectedValue),
                                      .SubjectID = CInt(vSubject.Value),
                                      .Level = CInt(cboLevel.SelectedValue)}
                vContext.programmesubjects.AddObject(ProgSubject)
            End If
        Next
        vContext.SaveChanges()
    End Sub

    Protected Sub SaveSubjects2()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vProgrammeSubject = (From p In vContext.programmesubjects Where p.QualID = CInt(cboQualification.SelectedValue) Select p)
        If vProgrammeSubject.Count > 0 Then
            For Each prog As programmesubject In vProgrammeSubject
                vContext.DeleteObject(prog)
            Next
        End If

    End Sub


    Function getOldStr(ByVal subjectid As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim oldcodes = (From p In vContext.oldsubjectcodes Where p.SubjectID = subjectid Select p).ToList
        Dim oStr As String = ""
        For Each ox In oldcodes
            If oStr = "" Then
                oStr = " -->" + ox.OldCode
            Else
                oStr = oStr + ", " + ox.OldCode
            End If
        Next
        Return oStr
    End Function

#End Region

#Region "Core Subjects"
    Sub loadCoreSubjects(ByVal vSearch As String, DepartID As Integer)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim QualExist As Boolean = CBool(IIf(cboQualification.SelectedIndex > -1, True, False))
        Dim CoreSubjects = (From p In vContext.subjects
                                           Order By p.longName
                                             Where QualExist And p.DepartmentID = DepartID And p.longName.Contains(vSearch)
                                                Select p).ToList
        With lstCoreSubjects
            .Items.Clear()
            For Each x In CoreSubjects
                Dim ostr = getOldStr(x.ID)
                Dim vItem As New ListItem(x.longName + " [" + x.Code + "]" + oStr, CStr(x.ID))
                .Items.Add(vItem)
            Next
        End With
    End Sub

    Protected Sub btnCoreAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCoreAdd.Click
        If lstCoreSubjects.SelectedIndex > -1 Then
            lstSelectedCoreSubjects.SelectedIndex = -1
            If lstSelectedCoreSubjects.Items.IndexOf(lstCoreSubjects.SelectedItem) = -1 Then
                lstSelectedCoreSubjects.Items.Add(lstCoreSubjects.SelectedItem)
            End If
        End If
    End Sub

    Protected Sub btnCoreRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCoreRemove.Click
        If lstSelectedCoreSubjects.SelectedIndex > -1 Then
            lstSelectedCoreSubjects.Items.RemoveAt(lstSelectedCoreSubjects.SelectedIndex)
        End If
    End Sub
#End Region

#Region "Service Subjects"


    Sub loadServiceSubjects(ByVal vSearch As String)
        Dim vContext As timetableEntities = New timetableEntities()
        Dim DepartID As Integer = getDepartment1.getID
        Dim QualExist As Boolean = CBool(IIf(cboQualification.SelectedIndex > -1, True, False))
        Dim serviceSubjects = (From p In vContext.subjects
                                            Order By p.longName
                                                Where QualExist And p.DepartmentID <> DepartID And
                                                      p.longName.Contains(vSearch)
                                                    Select p).ToList
        With lstServiceSubjects
            .Items.Clear()
            For Each x In serviceSubjects
                Dim ostr = getOldStr(x.ID)
                Dim vItem As New ListItem(x.longName + "[" + x.Code + "]" + ostr, CStr(x.ID))
                .Items.Add(vItem)
            Next
        End With
    End Sub

    Protected Sub btnServiceAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnServiceAdd.Click
        If lstServiceSubjects.SelectedIndex > -1 Then
            lstSelectedServiceSubject.SelectedIndex = -1
            If lstSelectedServiceSubject.Items.IndexOf(lstServiceSubjects.SelectedItem) = -1 Then
                lstSelectedServiceSubject.Items.Add(lstServiceSubjects.SelectedItem)
            End If
        End If
    End Sub

    Protected Sub btnServiceRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnServiceRemove.Click
        If lstSelectedServiceSubject.SelectedIndex > -1 Then
            lstSelectedServiceSubject.Items.RemoveAt(lstSelectedServiceSubject.SelectedIndex)
        End If
    End Sub
#End Region

#Region "Cluster Site"
    Sub loadAllClusters()
        lstAllClusters.Items.Clear()
        Dim vContext As timetableEntities = New timetableEntities()
        lstAllClusters.DataSource = (From p In vContext.siteclusters
                                       Order By p.longName
                                           Select p.longName, p.ID)
        lstAllClusters.DataTextField = "longName"
        lstAllClusters.DataValueField = "ID"
        lstAllClusters.DataBind()
    End Sub

    Protected Sub btnClusterAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClusterAdd.Click
        If lstAllClusters.SelectedIndex > -1 Then
            lstSelectedClusters.SelectedIndex = -1
            If lstSelectedClusters.Items.IndexOf(lstAllClusters.SelectedItem) = -1 Then
                lstSelectedClusters.Items.Add(lstAllClusters.SelectedItem)
            End If
        End If
    End Sub

    Protected Sub btnClusterRemove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClusterRemove.Click
        If lstSelectedClusters.SelectedIndex > -1 Then
            lstSelectedClusters.Items.RemoveAt(lstSelectedClusters.SelectedIndex)
        End If
    End Sub


    Protected Sub SaveSiteProgramme()
        Try
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vQual = (From p In vContext.qualifications Where p.ID = CInt(cboQualification.SelectedValue) Select p).First
            ' delete all those that are not selected
            Dim ClusterDelArr As New ArrayList
            If vQual.siteclusters.Count > 0 Then
                'mark those that need to be deleted
                For Each vCluster As sitecluster In vQual.siteclusters
                    Dim ClusterFound As Boolean = False
                    For Each vSelCluster As ListItem In lstSelectedClusters.Items
                        If vCluster.ID = CInt(vSelCluster.Value) Then
                            ClusterFound = True
                            Exit For
                        End If
                    Next
                    If Not ClusterFound Then
                        ClusterDelArr.Add(vCluster.ID)
                    End If
                Next
                'delete those that are not in the selected list
                For Each vclu As Integer In ClusterDelArr
                    Dim ClusterID As Integer = vclu
                    Dim cluster As sitecluster = (From p In vQual.siteclusters Where p.ID = ClusterID Select p).First
                    vQual.siteclusters.Remove(cluster)
                Next
            End If
            For Each vCluster As ListItem In lstSelectedClusters.Items
                'check if cluster exists 
                Dim vID As Integer = CInt(vCluster.Value)
                Dim clusterSearch = From p In vQual.siteclusters Where p.ID = vID Select p
                If clusterSearch.Count = 0 Then
                    Dim vNewCluster As sitecluster = (From p In vContext.siteclusters
                                                        Where p.ID = vID
                                                            Select p).First
                    vQual.siteclusters.Add(vNewCluster)
                End If
            Next
            vContext.SaveChanges()
            errorMessage.Text = clsGeneral.displaymessage("Updated!!", False)
        Catch ex As Exception
            errorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub
#End Region

    Private Sub getDepartment1_DepartmentClick(E As Object, Args As clsDepartmentEvent) Handles getDepartment1.DepartmentClick
        loadQualification(Args.mDepartmentID)
        loadCoreSubjects("", Args.mDepartmentID)
    End Sub

   
    Private Sub logSave_Click(sender As Object, e As System.EventArgs) Handles logSave.Click
        logSave.Function = "save"
        logSave.Description = "ID" + cboQualification.SelectedValue + "---Name" + cboQualification.SelectedItem.Text
        SaveSiteProgramme()
        SaveSubjects()
        displayQualificationDetails()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As System.EventArgs) Handles btnRefresh.Click
        displayQualificationDetails()
    End Sub
End Class