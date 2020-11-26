namespace Lab2

module XMLParsers =
    open FSharp.Data
    open System.Xml
    open System.IO

    type ParseType =
        | SAX
        | DOM
        | LINQ

    [<Literal>]
    let CustomersXmlSample = """
                <Customers>
                  <Customer name="ACME">
                    <Order Number="A012345">
                      <OrderLine Item="widget" Quantity="1"/>
                    </Order>
                    <Order Number="A012346">
                      <OrderLine Item="trinket" Quantity="2"/>
                    </Order>
                  </Customer>
                  <Customer name="Southwind">
                    <Order Number="A012347">
                      <OrderLine Item="skyhook" Quantity="3"/>
                      <OrderLine Item="gizmo" Quantity="4"/>
                    </Order>
                  </Customer>
                </Customers>"""

    [<Literal>]
    let OrderLinesXmlSample = """
    <html>
        <head>
            <title>XML Parser</title>
        </head>
        <body>
            <ul>
                <li>ACME
                    <ul>
                        <li>Order: A012345
                            <ul>
                                <li>
                                    Item: widget
                                    Quantity: 1
                                </li>
                            </ul>
                        </li>
                        <li>Order: A012346
                            <ul>
                                <li>
                                    Item: trinket
                                    Quantity: 2
                                </li>
                            </ul>
                        </li>
                    </ul>
                </li>
                <li>Southwind
                    <ul>
                        <li>Order: A012347
                            <ul>
                                <li>
                                    Item: skyhook
                                    Quantity: 3
                                </li>
                                <li>
                                    Item: gizmo
                                    Quantity: 4
                                </li>
                            </ul>
                        </li>

                    </ul>
                </li>
            </ul>

        </body>
    </html>"""

    type InputXml = XmlProvider<CustomersXmlSample>

    type OutputXml = XmlProvider<OrderLinesXmlSample>

    let parseLINQold xmlsrc =
        let xml = File.ReadAllText xmlsrc
        let realXML = InputXml.Parse xml
        let head = OutputXml.GetSample().Head

        let body =
            OutputXml.Body
                (OutputXml.Ul [| for customer in realXML.Customers do
                                     yield
                                         OutputXml.Li
                                             (OutputXml.Ul2 [| for order in customer.Orders do
                                                                   yield
                                                                       OutputXml.Li2
                                                                           (OutputXml.Ul3 [| for line in order.OrderLines do
                                                                                                 yield
                                                                                                     sprintf "Item: %s\nQuantity: %i"
                                                                                                         line.Item
                                                                                                         line.Quantity |]) |]) |])

        let html = OutputXml.Html(head, body)
        html |> sprintf "%A"

    let parseLINQ xmlsrc =
        // let xml = File.ReadAllText xmlsrc
        // let realXML = InputXml.Parse xml
        let realXML = InputXml.Parse xmlsrc
        Array.reduce
            (sprintf "%s\n\n%s")
            [| for customer in realXML.Customers do
                yield
                    sprintf "Customer: %s\n%s" customer.Name
                        ([| for order in customer.Orders do
                                yield
                                    [| for line in order.OrderLines do
                                        yield sprintf "\t\tItem: %s\n\t\tQuantity: %i" line.Item line.Quantity |]
                                    |> Array.reduce (sprintf "%s\n%s")
                                    |> sprintf "\tOrder: %s\n%s" order.Number |]
                         |> Array.reduce (sprintf "%s\n%s")) |]



    let parseSAX (xml: string) =
        let reader = XmlReader.Create xml
        [| while reader.Read() do
            if reader.NodeType = XmlNodeType.Element then
                yield reader.Name
                while reader.MoveToNextAttribute() do
                    yield sprintf "%s = %s" reader.Name reader.Value
            else
                yield sprintf "/%s" reader.Name |]
        |> Array.reduce (sprintf "%s\n%s")

    let parseDOM (xml: string) =
        let doc = XmlDocument()
        doc.LoadXml xml

        let src =
            [| for customer in doc.DocumentElement.ChildNodes do
                yield
                    sprintf "%s: %s\n%s" customer.Name (customer.Attributes.ItemOf(0).Value)
                        ([| for order in customer.ChildNodes do
                                yield
                                    sprintf "\t%s: %s\n%s" order.Name (order.Attributes.ItemOf(0).Value)
                                        ([| for orderLine in order.ChildNodes do
                                                yield
                                                    ([| for attr in orderLine.Attributes do
                                                            yield sprintf "\t\t%s: %s" attr.Name attr.Value |]
                                                     |> Array.reduce (sprintf "%s\n%s")) |]
                                         |> Array.reduce (sprintf "%s\n%s")) |]
                         |> Array.reduce (sprintf "%s\n%s")) |]

        Array.reduce (sprintf "%s\n\n%s") src

    let parseXML =
        function
        | SAX -> parseSAX
        | DOM -> parseDOM
        | LINQ -> parseLINQ
