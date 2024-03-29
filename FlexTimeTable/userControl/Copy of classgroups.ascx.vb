﻿Imports System.Web.HttpContext
Public Class classgroups
    Inherits System.Web.UI.UserControl

    Private TabHeader() As String = {"Class Groups", "Class", "Class Resources"}
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            loadOffering()
            loadTimeSlots()
            loadAllClusters()
            btnSave.Text = "Save"
            btnDelete.Text = "Delete"
        End If
    End Sub

    Public Sub setSubject(ByVal vSubjectID As Integer, vEditAccess As Boolean)
        ViewState("SubjectID") = CStr(vSubjectID)
        ViewState("EditAccess") = CStr(vEditAccess)
        btnCreateClass.Visible = vEditAccess
        btnClusterEdit.Visible = vEditAccess
        mvCluster.SetActiveView(vwClusterView)
        litMessage.Text = ""
        loadBlock()
        createSiteClusterSubjects()
        loadAssociatedQual()
        loadSelectedClusters()
        LoadClasses()
        changeMode(eMode.reset)
        ''''''''''''''''''''set classs   set  cluster
    End Sub

    Private Function GetSubjectID() As Integer
        Return CInt(ViewState("SubjectID"))
    End Function


    Private Function GetSiteClusterSubjectID() As Integer
        Dim vSubjID = GetSubjectID()
        Dim vClusterID = CInt(cboCluster.SelectedValue)
        Dim vContext As timetableEntities = New timetableEntities()
        Return (From p In vContext.siteclustersubjects
                                   Where p.SubjectID = vSubjID And
                                         p.SiteClusterID = vClusterID
                                      Select p.sitecluster).First.ID
    End Function

    Private Function IsEditAccess() As Boolean
        If CBool(ViewState("EditAccess")) Then
            Return True
        Else
            Return False
        End If
    End Function

    Sub createSiteClusterSubjects()
        Dim vSubjID = GetSubjectID()
        'using qualifications clusters and qualifications subjects ''''create siteclustersubjects
        Dim vContext As timetableEntities = New timetableEntities()
        For Each x In (From p In vContext.programmesubjects Where p.SubjectID = vSubjID Select p).ToList
            Dim vQual = x.qualification
            For Each y In (From p In vQual.siteclusters Select p).ToList
                Dim vClusterID = y.ID
                Dim vClusterSubject = (From p In vContext.siteclustersubjects
                                          Where p.SubjectID = vSubjID And
                                               p.SiteClusterID = vClusterID
                                                  Select p).FirstOrDefault
                If IsNothing(vClusterSubject) Then
                    vClusterSubject = New siteclustersubject With {
                           .SiteClusterID = vClusterID,
                           .SubjectID = vSubjID}
                    vContext.siteclustersubjects.AddObject(vClusterSubject)
                    vContext.SaveChanges()
                End If
            Next
        Next
    End Sub

    Sub loadAllClusters()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusters = (From p In vContext.siteclusters Order By p.longName Select name = p.longName, ID = p.ID).ToList
        lstAllClusters.Items.Clear()
        For Each x In vClusters
            Dim vItem As New ListItem(x.name, CStr(x.ID))
            lstAllClusters.Items.Add(vItem)
        Next
    End Sub

    Sub loadSelectedClusters()
        Dim vSubjID = GetSubjectID()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusterSubjects = (From p In vContext.siteclustersubjects Where p.SubjectID = vSubjID Select p.sitecluster).ToList
        Dim vClusters = (From p In vClusterSubjects Order By p.longName Select name = p.longName, ID = p.ID).ToList
        cboCluster.Items.Clear()
        lstSelClusters.Items.Clear()
        For Each x In vClusters
            Dim vItem As New ListItem(x.name, CStr(x.ID))
            cboCluster.Items.Add(vItem)
            lstSelClusters.Items.Add(vItem)
        Next
    End Sub




    Sub loadAssociatedQual()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubjectID = GetSubjectID()
        Dim vQual = (From p In vContext.programmesubjects Where p.SubjectID = vSubjectID Select p.qualification).ToList
        With lstQualification
            .Items.Clear()
            .DataSource = (From p In vQual Select ID = p.ID, name = p.longName + " [" + p.Code + "]").ToList
            .DataTextField = "name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub



    Sub LoadClasses()
        litMessage.Text = ""
        grdClasses.DataSource = Nothing
        If cboCluster.Items.Count > 0 Then
            btnCreateClass.Enabled = True
            Dim vContext As timetableEntities = New timetableEntities()
            Dim vSiteClusterID = CType(cboCluster.SelectedItem.Value, Integer)
            Dim vSubjectID = GetSubjectID()
            Dim vClasses = (From p In vContext.siteclustersubjects
                                Where p.SubjectID = vSubjectID And
                                      p.SiteClusterID = vSiteClusterID
                                      Select p.classgroups).FirstOrDefault
            If Not IsNothing(vClasses) Then
                grdClasses.DataSource = From p In vClasses Select p
            End If
        Else
            btnCreateClass.Enabled = False
            If btnClusterEdit.Visible Then
                pnlClass.Visible = False
                mvCluster.SetActiveView(vwClusterEdit)
            End If
        End If
        grdClasses.DataBind()
    End Sub

    Enum eMode
        edit
        reset
        create
        view
    End Enum

    Sub changeMode(ByVal vMode As eMode)
        Select Case vMode
            Case eMode.edit
                mvClass.ActiveViewIndex = 1
                pnlClassDetail.Enabled = True
                txtCode.Enabled = True
                cboOffering.Enabled = True
                cboBlock.Enabled = True
                txtSize.Enabled = True
                cboTimeSlots.Enabled = True
                btnCancel.Visible = True
                btnEdit.Visible = False
                btnDelete.Visible = True
                btnSave.Visible = True
                btnSave.Text = "Update"
                ucClassLecturer.Visible = True
                ucClassResource.Visible = True
                ucClassLecturer.setView(False)
                ucClassResource.SetView(False)
            Case eMode.create
                mvClass.ActiveViewIndex = 1
                pnlClassDetail.Enabled = True
                lblID.Text = ""
                txtCode.Text = ""
                txtSize.Text = "" '.shortName
                cboTimeSlots.SelectedIndex = 0
                cboOffering.SelectedIndex = 0
                cboBlock.SelectedIndex = 0
                txtCode.Enabled = True
                cboOffering.Enabled = True
                cboBlock.Enabled = True
                txtSize.Enabled = True
                cboTimeSlots.Enabled = True
                btnCancel.Visible = False
                btnEdit.Visible = False
                btnDelete.Visible = False
                btnSave.Visible = True
                btnSave.Text = "Save"
                ucClassLecturer.Visible = False
                ucClassResource.Visible = False
            Case eMode.reset
                mvClass.ActiveViewIndex = 0
                pnlClassDetail.Enabled = False
            Case eMode.view
                mvClass.ActiveViewIndex = 1
                pnlClassDetail.Enabled = False
                txtCode.Enabled = False
                cboOffering.Enabled = False
                cboBlock.Enabled = False
                txtSize.Enabled = False
                cboTimeSlots.Enabled = False
                btnCancel.Visible = False
                btnEdit.Visible = IsEditAccess()
                btnDelete.Visible = False
                btnSave.Visible = False
                btnSave.Text = "Save"
                ucClassLecturer.Visible = True
                ucClassResource.Visible = True
                ucClassLecturer.setView(True)
                ucClassResource.SetView(True)
        End Select
    End Sub

    Private Sub btnCreateClass_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateClass.Click
        changeMode(eMode.create)
        ucClassLecturer.mClassID = 0
        ucClassLecturer.mSubjectID = GetSubjectID()
        ucClassLecturer.LoadLecturer()
        litMessage.Text = ""
    End Sub


    Private Sub grdClasses_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdClasses.SelectedIndexChanged
        Dim vid = CInt(grdClasses.SelectedDataKey.Values(0))
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass As classgroup = _
                   (From p In vContext.classgroups _
                       Where p.ID = vid Select p).First
        With vClass
            lblID.Text = .ID.ToString
            txtCode.Text = .code
            txtSize.Text = .classSize.ToString
            Try
                cboBlock.SelectedValue = .AcademicBlockID.ToString
            Catch ex As Exception
            End Try
            cboOffering.SelectedValue = .OfferingTypeID.ToString
            cboTimeSlots.SelectedIndex = .TimeSlotTotal - 1
        End With
        loadClassControls(CInt(lblID.Text), True)
        changeMode(eMode.view)
        litMessage.Text = ""
    End Sub

    Sub loadClassControls(ByVal vid As Integer, ByVal ViewOnly As Boolean)
        'load class resource
        ucClassResource.ClassID = vid
        ucClassResource.LoadResources()

        'load Lectuerer
        ucClassLecturer.mClassID = vid
        ucClassLecturer.mSubjectID = GetSubjectID()
        ucClassLecturer.LoadLecturer()
       
    End Sub


    Function displayBlock(ByVal vBLockID As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim Block = (From p In vContext.academicblocks Where p.ID = vBLockID Select p).First
        Return Block.Name
    End Function

    Function displayOffering(ByVal vID As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim Offering = (From p In vContext.offeringtypes Where p.ID = vID Select p).First
        Return Offering.name
    End Function

    Sub loadBlock()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubjID = GetSubjectID()
        Dim vSubj = (From p In vContext.subjects Where p.ID = vSubjID Select p).Single
        Dim isYear = vSubj.yearBlock
        With cboBlock
            .DataSource = (From p In vContext.academicblocks Where p.yearBlock = isYear Select p.Name, p.ID).ToList
            .DataTextField = "Name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub loadOffering()
        Dim vContext As timetableEntities = New timetableEntities()
        With cboOffering
            .DataSource = (From p In vContext.offeringtypes Select p.name, p.ID).ToList
            .DataTextField = "Name"
            .DataValueField = "ID"
            .DataBind()
        End With
    End Sub

    Sub loadTimeSlots()
        With cboTimeSlots
            For i = 1 To 12
                .Items.Add(i.ToString)
            Next
        End With
    End Sub

    Function isClassCodeValid(ByVal vCode As String, ByVal vClassID As Integer, vSiteClusterID As Integer, vSubjectID As Integer) As Boolean
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass As classgroup =
            (From p In vContext.classgroups
                Where p.code = vCode And
                      p.SiteClusterID = vSiteClusterID And
                      p.SubjectID = vSubjectID
                            Select p).FirstOrDefault
        If IsNothing(vClass) Then
            Return True
        Else
            If vClass.ID = vClassID Then
                Return True
            Else
                Return False
            End If
        End If
    End Function


    Protected Function CreateClass() As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSiteClusterID = CType(cboCluster.SelectedItem.Value, Integer)
        Dim vSubjectID = GetSubjectID()

        Dim vSiteSubject = (From p In vContext.siteclustersubjects
                            Where p.SubjectID = vSubjectID And
                                  p.SiteClusterID = vSiteClusterID
                                  Select p).FirstOrDefault
        If IsNothing(vSiteSubject) Then
            'create 
            vSiteSubject = New siteclustersubject With {
                            .SiteClusterID = vSiteClusterID,
                            .SubjectID = vSubjectID}
            vContext.siteclustersubjects.AddObject(vSiteSubject)
            vContext.SaveChanges()
        End If

        If Not isClassCodeValid(txtCode.Text, 0, vSiteClusterID, vSubjectID) Then
            Throw New OverflowException("Code already exists for this Subject and Cluster")
        End If

        Dim vClass = New classgroup With {
            .code = txtCode.Text,
            .classSize = CInt(txtSize.Text),
            .AcademicBlockID = CInt(cboBlock.SelectedValue),
            .OfferingTypeID = CInt(cboOffering.SelectedValue),
            .TimeSlotTotal = cboTimeSlots.SelectedIndex + 1,
            .SiteClusterID = CType(cboCluster.SelectedItem.Value, Integer),
            .SubjectID = vSubjectID}
        vSiteSubject.classgroups.Add(vClass)
        vContext.SaveChanges()
        ucClassResource.ClassID = vClass.ID
        ucClassResource.createFirstResource()
        Return vClass.ID
    End Function

    Protected Sub UpdateClass()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSiteClusterID = CType(cboCluster.SelectedItem.Value, Integer)
        Dim vSubjectID = GetSubjectID()
        Dim vClassID = CType(lblID.Text, Integer)
        If Not isClassCodeValid(txtCode.Text, vClassID, vSiteClusterID, vSubjectID) Then
            Throw New OverflowException("Code already exists for this Subject and Cluster")
        End If
        Dim vClass As classgroup = _
            (From p In vContext.classgroups _
                Where p.ID = vClassID _
                    Select p).First
        With vClass
            .code = txtCode.Text
            .classSize = CInt(txtSize.Text)
            .AcademicBlockID = CInt(cboBlock.SelectedValue)
            .OfferingTypeID = CInt(cboOffering.SelectedValue)
            .TimeSlotTotal = cboTimeSlots.SelectedIndex + 1
        End With
        vContext.SaveChanges()
    End Sub

    Protected Sub DeleteClass()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClass As classgroup = _
             (From p In vContext.classgroups _
                 Where p.ID = CType(lblID.Text, Integer) _
                     Select p).First
        vContext.DeleteObject(vClass)
        vContext.SaveChanges()
    End Sub


    Function validAlphaNumerical(ByVal vText As String) As Boolean
        Return Regex.IsMatch(vText, "^[a-zA-Z0-9]+$")
    End Function

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            If Not validAlphaNumerical(txtCode.Text) Then
                Throw New OverflowException("A Valid Class Group Code is Required!!")
            End If
            If lblID.Text = "" Then
                lblID.Text = CStr(CreateClass())
                'changeMode(eMode.reset)
            Else
                UpdateClass()
            End If
            LoadClasses()
            loadClassControls(CInt(lblID.Text), True)
            changeMode(eMode.view)
            editMessage.Text = ""
        Catch ex As Exception
            If IsNothing(ex.InnerException) Then
                editMessage.Text = clsGeneral.displaymessage(ex.Message, True)
            Else
                editMessage.Text = clsGeneral.displaymessage(ex.InnerException.Message, True)
            End If
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try
            DeleteClass()
            LoadClasses()
            changeMode(eMode.reset)
            litMessage.Text = ""
        Catch ex As Exception
            If IsNothing(ex.InnerException) Then
                litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
            Else
                litMessage.Text = clsGeneral.displaymessage(ex.InnerException.Message, True)
            End If
        End Try
    End Sub


    Private Sub btnCancel_Click(sender As Object, e As System.EventArgs) Handles btnCancel.Click
        litMessage.Text = ""
        Try
            changeMode(eMode.view)
        Catch ex As Exception
            changeMode(eMode.reset)
        End Try
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As System.EventArgs) Handles btnEdit.Click
        loadClassControls(CInt(lblID.Text), False)
        changeMode(eMode.edit)
    End Sub


    Private Sub cboCluster_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboCluster.SelectedIndexChanged
        LoadClasses()
        changeMode(eMode.reset)
    End Sub

    Private Sub btnReturn_Click(sender As Object, e As System.EventArgs) Handles btnReturn.Click
        changeMode(eMode.reset)
    End Sub

    Private Sub classgroups_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        LoadClasses()
    End Sub

    Protected Sub btnClusterEdit_Click(sender As Object, e As EventArgs) Handles btnClusterEdit.Click
        pnlClass.Visible = False
        mvCluster.SetActiveView(vwClusterEdit)
    End Sub

    Protected Sub btnClusterReturn_Click(sender As Object, e As EventArgs) Handles btnClusterReturn.Click
        pnlClass.Visible = True
        mvCluster.SetActiveView(vwClusterView)
    End Sub

    Protected Sub btnAddCluster_Click(sender As Object, e As EventArgs) Handles btnAddCluster.Click
        'check if selected
        If lstAllClusters.SelectedIndex < 0 Then
            Exit Sub
        End If

        Dim NewClusterID = CInt(lstAllClusters.SelectedValue)
        'check if already exists
        If Not IsNothing(lstSelClusters.Items.FindByValue(CStr(NewClusterID))) Then
            Exit Sub
        End If
       
        Dim vSubjID = GetSubjectID()
        Dim vContext As timetableEntities = New timetableEntities()

        Dim NewClusterSubject As New siteclustersubject With {
            .SubjectID = GetSubjectID(),
            .SiteClusterID = NewClusterID}
        Try
            vContext.siteclustersubjects.AddObject(NewClusterSubject)
            vContext.SaveChanges()
        Catch ex As UpdateException
            litMessage.Text = clsGeneral.displaymessage("Cluster already exists!", True)
        Catch ex As Exception
            litMessage.Text = ""
        End Try
        loadSelectedClusters()
    End Sub

    Protected Sub btnRemCluster_Click(sender As Object, e As EventArgs) Handles btnRemCluster.Click
        'check if selected
        If lstSelClusters.SelectedIndex < 0 Then
            Exit Sub
        End If

        Dim vClusterID = CInt(lstSelClusters.SelectedValue)

        Dim vSubjID = GetSubjectID()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vClusterSubject = (From p In vContext.siteclustersubjects
                                   Where p.SubjectID = vSubjID And
                                         p.SiteClusterID = vClusterID
                                    Select p).Single
        Try
            vContext.siteclustersubjects.DeleteObject(vClusterSubject)
            vContext.SaveChanges()
            litMessage.Text = ""
        Catch ex As UpdateException
            litMessage.Text = clsGeneral.displaymessage("Classes are tied to this Cluster!!", True)
        Catch ex As Exception
            litMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
        loadSelectedClusters()
    End Sub
End Class