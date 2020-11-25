<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:template match="Customers">
        <HTML>
            <BODY>
                <UI>
                    <xsl:apply-templates select="Customer"/>
                </UI>
            </BODY>
        </HTML>
    </xsl:template>
    <xsl:template match="Customer">
        <LI>
            <xsl:text>Customer: </xsl:text>
            <xsl:value-of select="@Name"/>
            <UI>
                <xsl:apply-templates select="Order"/>
            </UI>
        </LI>
    </xsl:template>
    <xsl:template match="Order">
        <LI>
            <xsl:text>Order: </xsl:text>
            <xsl:value-of select="@Number"/>
            <UI>
                <xsl:apply-templates select="OrderLine"/>
            </UI>
        </LI>
    </xsl:template>
    <xsl:template match="OrderLine">
        <LI>
            <xsl:text>Item: </xsl:text>
            <xsl:value-of select="@Item"/>
            <xsl:text>&#10;</xsl:text>
            <xsl:text>Quantity: </xsl:text>
            <xsl:value-of select="@Quantity"/>
        </LI>
    </xsl:template>
</xsl:stylesheet>