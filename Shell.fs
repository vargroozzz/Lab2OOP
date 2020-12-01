namespace Lab2


module Shell =
    open Elmish

    type State =
        /// store the child state in your main state
        { MainViewState: MainView.State
          SaveFileState: SaveFile.State }

    type Msg =
        | MainViewMsg of MainView.Msg
        | SaveFileMsg of SaveFile.Msg

    let init =
        let mainViewState = MainView.init
        let saveFileState = SaveFile.init
        { MainViewState = mainViewState
          SaveFileState = saveFileState },
        Cmd.none

    // module Utils =


    module Controller =
        open System.IO
        open System.Xml.Xsl

        let update (msg: Msg) (state: State): State * Cmd<_> =
            match msg with
            | MainViewMsg mainviewmsg ->
                match mainviewmsg with
                | MainView.GenerateHTML ->
                    let xslt = XslCompiledTransform()
                    xslt.Load("XMLToHTML.xsl")
                    let path = state.SaveFileState.ChosenFile
                    xslt.Transform(path, path.Replace(".xml", ".html"))
                    // File.WriteAllText(path, parseXML state.Type state.Xml)
                    state, Cmd.none
                | _ ->
                    let mainViewMsg =
                        MainView.Controller.update mainviewmsg state.MainViewState

                    { state with
                          MainViewState = mainViewMsg },
                    Cmd.none
            | SaveFileMsg savefilemsg ->
                match savefilemsg with
                | SaveFile.LoadMsg ->
                    let newState =
                        File.ReadAllText state.SaveFileState.ChosenFile

                    let mainViewMsg =
                        MainView.Controller.update (newState |> MainView.LoadXML) state.MainViewState

                    { state with
                          MainViewState = mainViewMsg },
                    Cmd.none
                | SaveFile.SaveMsg ->
                    File.WriteAllText
                        (state.SaveFileState.ChosenFile,
                         XMLParsers.filterXML state.MainViewState.Filter state.MainViewState.Xml)
                    state, Cmd.none
                | _ ->
                    let saveFileMsg =
                        SaveFile.Controller.update savefilemsg state.SaveFileState

                    { state with
                          SaveFileState = saveFileMsg },
                    Cmd.none

    module View =
        open Avalonia.Controls
        open Avalonia.FuncUI.DSL

        let view (state: State) (dispatch) =
            DockPanel.create [ DockPanel.children [ TabControl.create [ TabControl.tabStripPlacement Dock.Top
                                                                        TabControl.viewItems [ TabItem.create [ TabItem.header
                                                                                                                    "MainView"
                                                                                                                TabItem.content
                                                                                                                    (MainView.View.view
                                                                                                                        state.MainViewState
                                                                                                                         (MainViewMsg
                                                                                                                          >> dispatch)) ]
                                                                                               TabItem.create [ TabItem.header
                                                                                                                    "File"
                                                                                                                TabItem.content
                                                                                                                    (SaveFile.View.view
                                                                                                                        state.SaveFileState
                                                                                                                         (SaveFileMsg
                                                                                                                          >> dispatch)) ]
                                                                                               TabItem.create [ TabItem.header
                                                                                                                    "About"
                                                                                                                TabItem.content
                                                                                                                    (About.View.view.Value) ] ] ] ] ]

    /// This is the main window of your application
    /// you can do all sort of useful things here like setting heights and widths
    /// as well as attaching your dev tools that can be super useful when developing with
    /// Avalonia

    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish

    type MainWindow() as this =
        inherit HostWindow()

        do
            base.Title <- "My Excel"
            base.Width <- 1200.0
            base.Height <- 800.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            Program.mkProgram (fun () -> init) Controller.update View.view
            |> Program.withHost this
            |> Program.run
