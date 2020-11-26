<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:template match="Customers">
        <HTML>
            <BODY>
                <UL>
                    <xsl:apply-templates select="Customer"/>
                </UL>
            </BODY>
        </HTML>
    </xsl:template>
    <xsl:template match="Customer">
        <LI>
            <xsl:text>Customer: </xsl:text>
            <xsl:value-of select="@name"/>
            <UL>
                <xsl:apply-templates select="Order"/>
            </UL>
        </LI>
    </xsl:template>
    <xsl:template match="Order">
        <LI>
            <xsl:text>Order: </xsl:text>
            <xsl:value-of select="@Number"/>
            <UL>
                <xsl:apply-templates select="OrderLine"/>
            </UL>
        </LI>
    </xsl:template>
    <xsl:template match="OrderLine">
        <LI>
            <xsl:text>Item: </xsl:text>
            <xsl:value-of select="@Item"/>
            <xsl:text>&#xA;</xsl:text>
            <xsl:text>Quantity: </xsl:text>
            <xsl:value-of select="@Quantity"/>
        </LI>
    </xsl:template>
</xsl:stylesheet>