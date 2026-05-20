<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="yes" encoding="utf-8"/>
<xsl:template match="/">
<html>
<head>
    <title>
        <xsl:choose>
            <xsl:when test="main/package">
                <xsl:value-of select="main/package/unidad"/>
            </xsl:when>

            <xsl:otherwise>
                <xsl:value-of select="main/methods/procedure/unidad"/>
            </xsl:otherwise>
        </xsl:choose>
    </title>
</head>
<body>

<xsl:if test="main/package">
    <table cellpadding="3"
           width="90%"
           style="background-color:#FFF;border-collapse:collapse;">

        <tr>
            <th align="left"
                colspan="6"
                style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:11.0pt;font-family:Arial;font-weight:bold">

                Paquete:
                <xsl:value-of select="main/package/unidad"/>
            </th>
        </tr>

        <tr>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
                Fuente de Datos
            </td>

            <td colspan="5"
                style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                
                <xsl:value-of select="main/package/@fuente"/>
            </td>
        </tr>

        <tr>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
                Creado Por
            </td>

            <td colspan="5" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">

                <xsl:value-of select="main/package/autor"/>
            </td>
        </tr>

        <tr>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
                Fecha de Creación
            </td>

            <td colspan="5"
                style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">

                <xsl:value-of select="main/package/fecha"/>
            </td>
        </tr>

        <tr>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
                Descripción:
            </td>

            <td colspan="5" style="border:1px solid black;padding:5px">
                <pre style="font-size:10.0pt;font-family:Arial;font-weight:plain">
                    <xsl:value-of select="main/package/descripcion"/>
                </pre>
            </td>

        </tr>

        <!-- HISTORIAL -->

        <tr>
            <td colspan="6"
                align="center"
                style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold">

                Historial de Modificaciones

            </td>
        </tr>

        <tr>
            <th width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Fecha</center></th>
            <th width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Autor</center></th>
            <th width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Incidente</center></th>
            <th width="40%" colspan="3" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Descripción</center></th>
        </tr>

        <xsl:for-each select="main/package/historial/modificacion">

            <tr>
                <td width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                    <xsl:value-of select="@fecha"/>
                </td>

                <td width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                    <xsl:value-of select="@autor"/>
                </td>

                <td width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                    <xsl:value-of select="@inc"/>
                </td>
                
                <td width="20%" colspan="4" style="border:1px solid black;padding:5px">
                    <pre style="font-size:10.0pt;font-family:Arial;font-weight:plain">
                        <xsl:value-of select="."/>
                    </pre>
                </td>
            </tr>
        </xsl:for-each>
    </table>
    <br/><br/>
</xsl:if>
<!-- MÉTODOS -->

<xsl:for-each select="main/methods/procedure">

    <table cellpadding="3"
           width="90%"
           style="background-color:#FFF;border-collapse:collapse;">
        <tr>
            <th align="left" colspan="6" style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:11.0pt;font-family:Arial;font-weight:bold">

                Metodo:
                <xsl:value-of select="unidad"/>
            </th>
        </tr>
        
        <tr>
        <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
            Fuente de Datos
        </td>

        <td colspan="5"
            style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
            
            <xsl:value-of select="@fuente"/>
        </td>
        </tr>
        
        <tr>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
                Descripción:
            </td>

            <td colspan="5" style="border:1px solid black;padding:5px">
                <pre style="font-size:10.0pt;font-family:Arial;font-weight:plain">
                    <xsl:value-of select="descripcion"/>
                </pre>
            </td>
        </tr>

        <tr>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
                Creado Por
            </td>

            <td colspan="5" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">

                <xsl:value-of select="autor"/>
            </td>
        </tr>

        <tr>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold">
                Fecha de Creación
            </td>

            <td colspan="5"
                style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">

                <xsl:value-of select="fecha"/>
            </td>
        </tr>        
        
        <xsl:if test="normalize-space(retorno/@nombre) != ''">
            <tr>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Retorno</center></td>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Tipo</center></td>
                <td colspan="4" style="border:1px solid black;background:#DCDCDC;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Descripción</center></td>
            </tr>
            
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                <xsl:value-of select="retorno/@nombre"/>
            </td>
            <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                <xsl:value-of select="retorno/@tipo"/>
            </td>
            <td colspan="4" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                <xsl:value-of select="retorno"/>
            </td>
        </xsl:if>

        <!-- PARAMETROS -->

        <xsl:if test="parametros/param">

            <tr>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Parametro</center></td>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Tipo</center></td>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>In</center></td>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Out</center></td>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Default</center></td>
                <td style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Descripción</center></td>
            </tr>
            <xsl:for-each select="parametros/param">

                <tr>
                    <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                        <xsl:value-of select="@nombre"/>
                    </td>

                    <td style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                        <xsl:value-of select="@tipo"/>
                    </td>
                    
                    <xsl:choose>
                        <xsl:when test="@direccion = 'IN' or @direccion = 'In' or @direccion = 'in'">
                            <td align="center" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                                X
                            </td>
                        </xsl:when>

                        <xsl:otherwise>
                            <td align="center" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                            </td>
                        </xsl:otherwise>
                    </xsl:choose>
                    
                    <xsl:choose>
                        <xsl:when test="@direccion = 'OUT' or @direccion = 'Out' or @direccion = 'out'">
                            <td align="center" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                                X
                            </td>
                        </xsl:when>

                        <xsl:otherwise>
                            <td align="center" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                            </td>
                        </xsl:otherwise>
                    </xsl:choose>
                    
                    <xsl:choose>
                        <xsl:when test="normalize-space(@default) != ''">
                            <td align="center" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                                <xsl:value-of select="@default"/>
                            </td>
                        </xsl:when>

                        <xsl:otherwise>
                            <td align="center" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                            </td>
                        </xsl:otherwise>
                    </xsl:choose>

                    <td style="border:1px solid black;padding:2px;">
                        <pre style="font-size:10.0pt;font-family:Arial;font-weight:plain">
                            <xsl:value-of select="."/>
                        </pre>
                    </td>
                </tr>
            </xsl:for-each>
        </xsl:if>
        
        <!-- COMENTARIOS -->

        <xsl:if test="comentarios/com">
            <tr>
                <td colspan="6" align="center" style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold">
                    Comentarios Método
                </td>
            </tr>
            <xsl:for-each select="comentarios/com">
                <tr>
                    <td colspan="6" style="border:1px solid black;padding:2px;">
                        <pre style="font-size:10.0pt;font-family:Arial;font-weight:plain">
                            <xsl:value-of select="."/>
                        </pre>
                    </td>
                </tr>
            </xsl:for-each>
        </xsl:if>
        
        <!-- HISTORIAL -->
        <xsl:if test="historial/modificacion">
            <tr>
                <td colspan="6" align="center" style="border:1px solid black;padding:5px;background:#DCDCDC;font-size:10.0pt;font-family:Arial;font-weight:bold">

                    Historial de Modificaciones

                </td>
            </tr>

            <tr>
                <th width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Fecha</center></th>
                <th width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Autor</center></th>
                <th width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Incidente</center></th>
                <th width="40%" colspan="3" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:bold"><center>Descripción</center></th>
            </tr>

            <xsl:for-each select="historial/modificacion">

                <tr>
                    <tr>
                        <td width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                            <xsl:value-of select="@fecha"/>
                        </td>

                        <td width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                            <xsl:value-of select="@autor"/>
                        </td>

                        <td width="20%" style="border:1px solid black;padding:5px;font-size:10.0pt;font-family:Arial;font-weight:plain">
                            <xsl:value-of select="@inc"/>
                        </td>
                        
                        <td width="20%" colspan="4" style="border:1px solid black;padding:5px">
                            <pre style="font-size:10.0pt;font-family:Arial;font-weight:plain">
                                <xsl:value-of select="."/>
                            </pre>
                        </td>
                    </tr>
                </tr>
            </xsl:for-each>
        </xsl:if>
    </table>
    <br/><br/>
</xsl:for-each>
</body>
</html>
</xsl:template>
</xsl:stylesheet>
