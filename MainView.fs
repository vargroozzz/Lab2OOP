namespace Lab2

open System.Xml.Xsl

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
        | GenerateHTML of string

    module Utils =
        open Avalonia.Controls

        let mainWindow () =
            (Avalonia.Application.Current.ApplicationLifetime :?> ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime).MainWindow

    module Controller =


        let update (msg: Msg) (state: State): State =
            match msg with
            | SetToSAX -> { state with Type = SAX }
            | SetToDOM -> { state with Type = DOM }
            | SetToLINQ -> { state with Type = LINQ }
            | LoadXML newXML -> { state with Xml = newXML }
            | GenerateHTML path ->
                let xslt = XslCompiledTransform()
                xslt.Load("XMLToHTML.xsl")
                xslt.Transform(path, path.Replace(".xml", ".html"))
                // File.WriteAllText(path, parseXML state.Type state.Xml)
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
                            Button.width 100.0
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
                                                        controlButton "LINQ to XML" (fun _ -> dispatch SetToLINQ) ] ]

        let view (state: State) dispatch =
            DockPanel.create [ DockPanel.children [ controlButtons dispatch
                                                    TextBlock.create [ TextBlock.dock Dock.Top
                                                                       TextBlock.fontSize 14.0
                                                                       TextBlock.verticalAlignment VerticalAlignment.Top
                                                                       TextBlock.horizontalAlignment
                                                                           HorizontalAlignment.Left
                                                                       TextBlock.text (parseXML state.Type state.Xml) ] ] ]
