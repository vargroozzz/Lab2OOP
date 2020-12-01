namespace Lab2


module XMLParsers =
    open FSharp.Data
    open System.Xml
    open System.Xml.Linq
    open System.IO

    type ParseType =
        | SAX
        | DOM
        | LINQ

    type Filter =
        { Customer: string
          Order: string
          ItemName: string
          Quantity: int option }

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

    type InputXml = XmlProvider<CustomersXmlSample>

    let filterXML filter xml =
        let realXML = InputXml.Parse xml

        let lineFilter (line: InputXml.OrderLine) =
            (line.Item = filter.ItemName
             && Some(line.Quantity) = filter.Quantity)
            || (filter.ItemName = ""
                && Some(line.Quantity) = filter.Quantity)
            || (line.Item = filter.ItemName
                && filter.Quantity = None)
            || (filter.ItemName = "" && filter.Quantity = None)

        let orderFilter (order: InputXml.Order) =
            (order.Number = filter.Order || filter.Order = "")
            && Array.exists lineFilter order.OrderLines

        let customerFilter (customer: InputXml.Customer) =
            (customer.Name = filter.Customer
             || filter.Customer = "")
            && Array.exists orderFilter customer.Orders

        let filteredXML =
            InputXml.Customers [| for customer in realXML.Customers do
                                      if customerFilter customer then
                                          yield
                                              InputXml.Customer
                                                  (customer.Name,
                                                   [| for order in customer.Orders do
                                                       if orderFilter order then
                                                           yield
                                                               InputXml.Order
                                                                   (order.Number,
                                                                    [| for line in order.OrderLines do
                                                                        if lineFilter line then
                                                                            yield
                                                                                InputXml.OrderLine
                                                                                    (line.Item, line.Quantity) |]) |]) |]

        filteredXML.ToString()

    let parseLINQ filter xmlsrc =
        // let xml = File.ReadAllText xmlsrc
        // let realXML = InputXml.Parse xml

        let realXML =
            xmlsrc |> filterXML filter |> InputXml.Parse

        Array.fold
            (sprintf "%s\n\n%s")
            ""
            [| for customer in realXML.Customers do
                yield
                    sprintf "Customer: %s\n%s" customer.Name
                        ([| for order in customer.Orders do
                                yield
                                    [| for line in order.OrderLines do
                                        yield sprintf "\t\tItem: %s\n\t\tQuantity: %i" line.Item line.Quantity |]
                                    |> Array.fold (sprintf "%s\n%s") ""
                                    |> sprintf "\tOrder: %s\n%s" order.Number |]
                         |> Array.fold (sprintf "%s\n%s") "") |]



    let parseSAXOld filter (xml: string) =
        let reader =
            xml |> filterXML filter |> XmlReader.Create

        [| while reader.Read() do
            if reader.NodeType = XmlNodeType.Element then
                yield reader.Name
                while reader.MoveToNextAttribute() do
                    yield sprintf "%s = %s" reader.Name reader.Value
            else
                yield sprintf "/%s" reader.Name |]
        |> Array.reduce (sprintf "%s\n%s")


    let parseSAX filter xmlsrc =
        // let xml = File.ReadAllText xmlsrc
        // let realXML = InputXml.Parse xml

        let realXML =
            xmlsrc |> filterXML filter |> InputXml.Parse

        Array.fold
            (sprintf "%s\n\n%s")
            ""
            [| for customer in realXML.Customers do
                yield
                    sprintf "Customer: %s\n%s" customer.Name
                        ([| for order in customer.Orders do
                                yield
                                    [| for line in order.OrderLines do
                                        yield sprintf "\t\tItem: %s\n\t\tQuantity: %i" line.Item line.Quantity |]
                                    |> Array.fold (sprintf "%s\n%s") ""
                                    |> sprintf "\tOrder: %s\n%s" order.Number |]
                         |> Array.fold (sprintf "%s\n%s") "") |]

    let parseDOM filter (xml: string) =

        let doc = XmlDocument()
        xml |> filterXML filter |> doc.LoadXml

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
        // | SAX -> parseLINQ
        | DOM -> parseDOM
        | LINQ -> parseLINQ
