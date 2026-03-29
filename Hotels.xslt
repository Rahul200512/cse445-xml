<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" />
  <xsl:template match="/">
    <html>
      <body>
        <h2>Hotel List</h2>
        <table border="1">
          <tr>
            <th>Name</th>
            <th>Rating</th>
            <th>Phone</th>
            <th>Street</th>
            <th>City</th>
            <th>State</th>
            <th>Zip</th>
          </tr>
          <xsl:for-each select="Hotels/Hotel">
            <xsl:sort select="Name" order="ascending" />
            <tr>
              <td><xsl:value-of select="Name" /></td>
              <td><xsl:value-of select="@Rating" /></td>
              <td><xsl:value-of select="Phone" /></td>
              <td><xsl:value-of select="Address/Street" /></td>
              <td><xsl:value-of select="Address/City" /></td>
              <td><xsl:value-of select="Address/State" /></td>
              <td><xsl:value-of select="Address/Zip" /></td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
