namespace Lab2

module MainView =

    open XMLParsers
    open System.IO


    type State = { Type: ParseType; Xml: string }

    let init =
        { Type = LINQ
          Xml = File.ReadAllText "customers.xml" }

    type Msg =
        | SetToSAX
        | SetToDOM
        | SetToLINQ
        | LoadXML of string
        | GenerateHTML

    module Utils =
        open Avalonia.Controls
        open Avalonia.Media
        open Avalonia.Layout

        let mainWindow () =
            (Avalonia.Application.Current.ApplicationLifetime :?> ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime).MainWindow

    module Controller =


        let update (msg: Msg) (state: State): State =
            match msg with
            | SetToSAX -> { state with Type = SAX }
            | SetToDOM -> { state with Type = DOM }
            | SetToLINQ -> { state with Type = LINQ }
            | LoadXML newXML -> { state with Xml = newXML }
            | GenerateHTML ->
                // let xml = File.ReadAllText "customers.xml"
                let html = parseXML state.Type "customers.xml"
                File.WriteAllText("customers.html", html)
                state

    module View =
        open Avalonia.Controls
        open Avalonia.FuncUI.DSL
        open Avalonia.Layout
        open Avalonia.Controls.Primitives

        let controlButton (content: string) fn =
            Button.create [ Button.content content
                            Button.background "white"
                            Button.foreground "black"
                            Button.fontSize 14.0
                            Button.verticalAlignment VerticalAlignment.Center
                            Button.horizontalAlignment HorizontalAlignment.Center
                            Button.width 75.0
                            Button.height 25.0
                            Button.onClick fn ]

        let controlButtons dispatch =
            UniformGrid.create [ UniformGrid.width 400.0
                                 UniformGrid.height 300.0
                                 UniformGrid.rows 1
                                 UniformGrid.columns 4
                                 UniformGrid.horizontalAlignment HorizontalAlignment.Center
                                 UniformGrid.verticalAlignment VerticalAlignment.Top
                                 UniformGrid.children [ controlButton "SAX API" (fun _ -> dispatch SetToSAX)
                                                        controlButton "DOM API" (fun _ -> dispatch SetToDOM)
                                                        controlButton "LINQ to XML" (fun _ -> dispatch SetToLINQ)
                                                        controlButton "SaveHTML" (fun _ -> dispatch GenerateHTML)
                                                        controlButton "LoadHTML" (fun _ -> ()) ] ]

        let view (state: State) dispatch =
            DockPanel.create [ DockPanel.children [ controlButtons dispatch
                                                    TextBlock.create [ TextBlock.dock Dock.Top
                                                                       TextBlock.fontSize 48.0
                                                                       TextBlock.verticalAlignment
                                                                           VerticalAlignment.Center
                                                                       TextBlock.horizontalAlignment
                                                                           HorizontalAlignment.Center
                                                                       TextBlock.text (string state.Type) ] ] ]
