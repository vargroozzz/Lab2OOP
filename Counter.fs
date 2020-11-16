namespace Lab2

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout

    type ParseType =
        | SAX
        | DOM
        | LINQ

    type State = { Type: ParseType }
    let init = { Type = SAX }

    type Msg =
        | SetToSAX
        | SetToDOM
        | SetToLINQ

    let update (msg: Msg) (state: State): State =
        match msg with
        | SetToSAX -> { state with Type = SAX }
        | SetToDOM -> { state with Type = DOM }
        | SetToLINQ -> { state with Type = LINQ }

    let view (state: State) (dispatch) =
        DockPanel.create [ DockPanel.children [ Button.create [ Button.dock Dock.Bottom
                                                                Button.onClick (fun _ -> dispatch SetToSAX)
                                                                Button.content "SAX API" ]
                                                Button.create [ Button.dock Dock.Bottom
                                                                Button.onClick (fun _ -> dispatch SetToDOM)
                                                                Button.content "DOM API" ]
                                                Button.create [ Button.dock Dock.Bottom
                                                                Button.onClick (fun _ -> dispatch SetToLINQ)
                                                                Button.content "LINQ to XML" ]
                                                TextBlock.create [ TextBlock.dock Dock.Top
                                                                   TextBlock.fontSize 48.0
                                                                   TextBlock.verticalAlignment VerticalAlignment.Center
                                                                   TextBlock.horizontalAlignment
                                                                       HorizontalAlignment.Center
                                                                   TextBlock.text (string state.Type) ] ] ]
