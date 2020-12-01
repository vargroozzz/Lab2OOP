namespace Lab2

open System.Xml.Xsl

module MainView =

    open XMLParsers
    open System.IO

    type State =
        { Type: ParseType
          Xml: string
          Filter: XMLParsers.Filter }

    let init =
        { Type = LINQ
          Xml = File.ReadAllText "customers.xml"
          Filter =
              { Customer = ""
                Order = ""
                ItemName = ""
                Quantity = None } }

    type Msg =
        | SetToSAX
        | SetToDOM
        | SetToLINQ
        | CustomerFilter of string
        | OrderFilter of string
        | ItemNameFilter of string
        | QuantityFilter of int option
        | LoadXML of string
        | GenerateHTML

    module Utils =
        open Avalonia.Controls

        let mainWindow () =
            (Avalonia.Application.Current.ApplicationLifetime :?> ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime).MainWindow

        let tryParseInt: string -> int option =
            System.Int32.TryParse
            >> function
            | true, v -> Some v
            | false, _ -> None


    module Controller =


        let update (msg: Msg) (state: State): State =
            match msg with
            | SetToSAX -> { state with Type = SAX }
            | SetToDOM -> { state with Type = DOM }
            | SetToLINQ -> { state with Type = LINQ }
            | CustomerFilter filter ->
                { state with
                      Filter = { state.Filter with Customer = filter } }
            | OrderFilter filter ->
                { state with
                      Filter = { state.Filter with Order = filter } }
            | ItemNameFilter filter ->
                { state with
                      Filter = { state.Filter with ItemName = filter } }
            | QuantityFilter filter ->
                { state with
                      Filter = { state.Filter with Quantity = filter } }
            | LoadXML newXML -> { state with Xml = newXML }
    // | GenerateHTML path ->
    //     let xslt = XslCompiledTransform()
    //     xslt.Load("XMLToHTML.xsl")
    //     xslt.Transform(path, path.Replace(".xml", ".html"))
    //     // File.WriteAllText(path, parseXML state.Type state.Xml)
    //     state

    module View =
        open Avalonia.Controls
        open Avalonia.FuncUI.DSL
        open Avalonia.Layout
        open Avalonia.Controls.Primitives

        let filterView label row onChange =
            StackPanel.create [ Grid.row row
                                Grid.column 0
                                Grid.columnSpan 3
                                StackPanel.orientation Orientation.Horizontal
                                StackPanel.verticalAlignment VerticalAlignment.Center
                                StackPanel.children [ TextBlock.create [ TextBlock.text label
                                                                         TextBlock.horizontalAlignment
                                                                             HorizontalAlignment.Left
                                                                         TextBlock.width 100.0
                                                                         TextBlock.height 25.0 ]
                                                      TextBox.create [ TextBox.width 200.0
                                                                       TextBox.height 25.0
                                                                       TextBox.text ""
                                                                       TextBox.onTextChanged onChange ] ] ]

        let customerFilter dispatch =
            StackPanel.create [ Grid.row 1
                                Grid.column 0
                                Grid.columnSpan 3
                                StackPanel.orientation Orientation.Horizontal
                                StackPanel.verticalAlignment VerticalAlignment.Center
                                StackPanel.children [ TextBlock.create [ TextBlock.text "Customer: "
                                                                         TextBlock.horizontalAlignment
                                                                             HorizontalAlignment.Left
                                                                         TextBlock.width 110.0
                                                                         TextBlock.height 25.0 ]
                                                      TextBox.create [ TextBox.width 220.0
                                                                       TextBox.height 25.0
                                                                       TextBox.onTextChanged
                                                                           (CustomerFilter >> dispatch) ] ] ]

        let controlButton (content: string) column fn =
            Button.create [ Button.content content
                            Button.background "white"
                            Button.foreground "black"
                            Grid.row 0
                            Grid.column column
                            Button.fontSize 14.0
                            Button.verticalAlignment VerticalAlignment.Center
                            Button.horizontalAlignment HorizontalAlignment.Center
                            Button.width 110.0
                            Button.height 25.0
                            Button.onClick fn ]

        let controlButtons dispatch =
            Border.create [ Border.borderThickness 2.0
                            Border.borderBrush "black"
                            Border.padding 10.0
                            Border.margin (left = 0.0, top = 0.0, bottom = 0.0, right = 5.0)
                            Border.child
                                (Grid.create [ Grid.columnDefinitions "Auto,Auto,Auto,Auto"
                                               Grid.rowDefinitions "Auto,Auto,Auto,Auto,Auto"
                                               Grid.width 450.0
                                               Grid.height 300.0
                                               //    Grid.rows 1
                                               //    Grid.columns 4
                                               Grid.horizontalAlignment HorizontalAlignment.Center
                                               Grid.verticalAlignment VerticalAlignment.Top
                                               Grid.children [ controlButton "SAX API" 0 (fun _ -> dispatch SetToSAX)
                                                               controlButton "DOM API" 1 (fun _ -> dispatch SetToDOM)
                                                               controlButton "LINQ to XML" 2 (fun _ ->
                                                                   dispatch SetToLINQ)
                                                               controlButton "Generate HTML" 3 (fun _ ->
                                                                   dispatch GenerateHTML)
                                                               filterView "Customer: " 1 (CustomerFilter >> dispatch)
                                                               filterView "Order: " 2 (OrderFilter >> dispatch)
                                                               filterView "ItemName: " 3 (ItemNameFilter >> dispatch)
                                                               filterView
                                                                   "Quantity: "
                                                                   4
                                                                   (Utils.tryParseInt >> QuantityFilter >> dispatch) ] ]) ]

        let view (state: State) dispatch =
            DockPanel.create [ DockPanel.children [ controlButtons dispatch
                                                    TextBlock.create [ TextBlock.dock Dock.Top
                                                                       TextBlock.fontSize 14.0
                                                                       TextBlock.verticalAlignment VerticalAlignment.Top
                                                                       TextBlock.horizontalAlignment
                                                                           HorizontalAlignment.Left
                                                                       TextBlock.text
                                                                           (parseXML state.Type state.Filter state.Xml) ] ] ]
