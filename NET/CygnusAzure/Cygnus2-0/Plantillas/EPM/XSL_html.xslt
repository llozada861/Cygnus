<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:output method="html" indent="yes" encoding="utf-8"/>

<xsl:template match="/">

<html>

<head>

<meta charset="utf-8"/>
<meta name="viewport" content="width=device-width, initial-scale=1.0"/>

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

<style>

*{
    box-sizing:border-box;
}

html,
body{
    margin:0;
    padding:0;
    width:100%;
    overflow-x:hidden;
    font-family:Arial, Helvetica, sans-serif;
    background:#f4f6f8;
    color:#222;
}

/* =========================
   LAYOUT
========================= */

.sidebar{
    position:fixed;
    left:0;
    top:0;
    width:450px;
    height:100vh;
    background:#fff;
    border-right:1px solid #ddd;
    padding:15px;
    overflow-y:auto;
    z-index:1000;
}

.content{
    margin-left:450px;
    padding:20px;
    width:calc(100% - 450px);
    min-width:0;
}

/* =========================
   MENU
========================= */

.sidebar h3{
    margin-top:0;
    color:#333;
}

#tree{
    padding-left:18px;
    margin:0;
}

#tree ul{
    padding-left:20px;
}

#tree li{
    margin-left:5px;
}

#tree > li{
    list-style-type:circle;
}

#tree li li{
    list-style-type:circle;
}

#tree a{
    display:block;
    padding:8px 10px;
    margin:2px 0;
    border-radius:5px;
    text-decoration:none;
    color:#333;
    font-size:13px;
    line-height:1.4;
    word-break:break-word;
}

#tree a:hover{
    background:#629ef2;
}

.active-link{
    background:#0078D4;
    color:#fff !important;
}

/* =========================
   CARDS
========================= */

.card{
    width:100%;
    background:#fff;
    border:1px solid #ddd;
    border-radius:8px;
    overflow:hidden;
    margin-bottom:20px;
    min-width:0;
}

.card-header{
    background:#0078D4;
    color:#fff;
    padding:14px 16px;
    font-weight:bold;
    cursor:pointer;
    word-break:break-word;
}

.card-body{
    padding:15px;
    width:100%;
    min-width:0;
}

/* =========================
   TITULOS
========================= */

.hist-title{
    display:block;
    width:100%;
    margin:25px 0 10px 0;
    padding:12px 15px;
    background:#f5f7fa;
    color:#2c3e50;
    font-size:14px;
    font-weight:bold;
    border:1px solid #dcdfe6;
    border-left:5px solid #1f4e79;
    border-radius:4px;
    text-align:center;
    word-break:break-word;
}

/* =========================
   ROW GENERAL
========================= */

.row{
    display:grid;
    grid-template-columns:220px minmax(0,1fr);
    width:100%;
    border:1px solid #eee;
    border-radius:6px;
    overflow:hidden;
}

.row > div{
    padding:10px;
    border-bottom:1px solid #eee;
}

.row > div:nth-last-child(-n+2){
    border-bottom:none;
}

.label{
    background:#fafafa;
    font-weight:bold;
    color:#555;
    word-break:break-word;
}

.value{
    min-width:0;
    overflow-wrap:anywhere;
    word-break:break-word;
    white-space:pre-wrap;
}

/* =========================
   TABLAS RESPONSIVE
========================= */

.hist-header,
.hist-row,
.param-header,
.param-row,
.retorno-header,
.retorno-row{
    width:100%;
    display:grid;
    gap:0;
    border:1px solid #e5e5e5;
    border-top:none;
    min-width:0;
}

.hist-header:first-of-type,
.param-header:first-of-type,
.retorno-header:first-of-type{
    border-top:1px solid #dcdfe6;
}

.hist-header{
    grid-template-columns:
        120px
        150px
        150px
        minmax(0,1fr);

    background:#f5f7fa;
    font-weight:bold;
    color:#2c3e50;
}

.hist-row{
    grid-template-columns:
        120px
        150px
        150px
        minmax(0,1fr);

    background:#fff;
}

.param-header{
    grid-template-columns:
        180px
        150px
        70px
        70px
        150px
        minmax(0,1fr);

    background:#f5f7fa;
    font-weight:bold;
    color:#2c3e50;
}

.param-row{
    grid-template-columns:
        180px
        150px
        70px
        70px
        150px
        minmax(0,1fr);

    background:#fff;
}

.retorno-header{
    grid-template-columns:
        180px
        150px
        minmax(0,1fr);

    background:#f5f7fa;
    font-weight:bold;
    color:#2c3e50;
}

.retorno-row{
    grid-template-columns:
        180px
        150px
        minmax(0,1fr);

    background:#fff;
}

.hist-header > div,
.hist-row > div,
.param-header > div,
.param-row > div,
.retorno-header > div,
.retorno-row > div{
    padding:10px;
    border-right:1px solid #e5e5e5;
    min-width:0;
    overflow-wrap:anywhere;
    word-break:break-word;
    display:flex;
    align-items:flex-start;
}

.hist-header > div:last-child,
.hist-row > div:last-child,
.param-header > div:last-child,
.param-row > div:last-child,
.retorno-header > div:last-child,
.retorno-row > div:last-child{
    border-right:none;
}

.hist-header,
.param-header,
.retorno-header{
    text-align:center;
    font-size:13px;
}

.col{
    font-size:13px;
    min-width:0;
}

.descripcion{
    overflow-wrap:anywhere;
    word-break:break-word;
}

/* =========================
   PRE
========================= */

pre{
    margin:0;
    width:100%;
    white-space:pre-wrap;
    overflow-wrap:anywhere;
    word-break:break-word;
    font-family:Arial, Helvetica, sans-serif;
    font-size:13px;
}

/* =========================
   MOBILE
========================= */

@media (max-width: 1024px){

    .sidebar{
        width:240px;
    }

    .content{
        margin-left:240px;
        width:calc(100% - 240px);
    }
}

/* =========================
   COMENTARIOS METODO
========================= */

.comentarios-metodo{
    width:100%;
}

.comentario-row{
    width:100%;
    display:block;
    border:1px solid #e5e5e5;
    border-top:none;
    background:#fff;
}

.comentario-row:first-child{
    border-top:1px solid #e5e5e5;
}

.comentario-col{
    width:100%;
    padding:1px;
    min-width:0;
    overflow-wrap:anywhere;
    word-break:break-word;
    white-space:pre-wrap;
}

.comentario-col pre{
    width:100%;
    margin:0;
    white-space:pre-wrap;
    overflow-wrap:anywhere;
    word-break:break-word;
}

@media (max-width: 768px){

    .sidebar{
        position:relative;
        width:100%;
        height:auto;
        border-right:none;
        border-bottom:1px solid #ddd;
    }

    .content{
        margin-left:0;
        width:100%;
        padding:12px;
    }

    .row{
        grid-template-columns:1fr;
    }

    .row > div{
        border-bottom:1px solid #eee !important;
    }

    .hist-header,
    .param-header,
    .retorno-header{
        display:none;
    }

    .hist-row,
    .param-row,
    .retorno-row{
        grid-template-columns:1fr;
        border-radius:6px;
        margin-bottom:12px;
        overflow:hidden;
    }

    .hist-row > div,
    .param-row > div,
    .retorno-row > div{
        border-right:none;
        border-bottom:1px solid #eee;
        padding:10px;
    }

    .hist-row > div:last-child,
    .param-row > div:last-child,
    .retorno-row > div:last-child{
        border-bottom:none;
    }

    .card-body{
        padding:10px;
    }

    .card-header{
        font-size:14px;
        line-height:1.4;
    }

    .hist-title{
        font-size:13px;
    }
}

</style>

<script type="text/javascript">
<![CDATA[

function toggleSection(id){

    var all = document.querySelectorAll(".card-body");

    for(var i = 0; i < all.length; i++){
        all[i].style.display = "none";
    }

    var elements = document.getElementsByClassName(id);

    for(var j = 0; j < elements.length; j++){
        elements[j].style.display = "block";
    }
}

document.addEventListener("DOMContentLoaded", function(){

    document.querySelectorAll("#tree a").forEach(function(a){

        a.addEventListener("click", function(){

            document.querySelectorAll("#tree a").forEach(function(x){
                x.classList.remove("active-link");
            });

            this.classList.add("active-link");
        });
    });
});

function openSection(id){

    var allRows = document.querySelectorAll("[class^='met_'], [class^='pkg_']");

    for(var i = 0; i < allRows.length; i++){
        allRows[i].style.display = "none";
    }

    var rows = document.getElementsByClassName(id);

    for(var j = 0; j < rows.length; j++){
        rows[j].style.display = "block";
    }
}

]]>
</script>

</head>

<body>

<div class="sidebar">

    <h3>Navegación</h3>

    <xsl:if test="main/package or main/methods/procedure">

        <ul id="tree">

            <xsl:if test="main/package">
                <li>
                    <a href="#pkg" onclick="openSection('pkg_1')">
                        <b><xsl:value-of select="main/package/unidad"/></b>
                        :
                        <xsl:value-of select="main/package/descripcion"/>
                    </a>
                </li>
            </xsl:if>

            <xsl:for-each select="main/methods/procedure">

                <li>
                    <a href="#met_{position()}" onclick="openSection('met_{position()}')">
                        <b><xsl:value-of select="unidad"/></b>
                        :
                        <xsl:value-of select="descripcion"/>
                    </a>
                </li>

            </xsl:for-each>

        </ul>

    </xsl:if>

</div>

<div class="content">

    <!-- PACKAGE -->
    <xsl:if test="main/package">

        <div class="card" id="pkg">

            <div class="card-header" onclick="toggleSection('pkg_1')">
                Paquete:
                <xsl:value-of select="main/package/unidad"/>
            </div>

            <div class="card-body pkg_1" style="display:none;">

                <div class="row">

                    <div class="label">Fuente de Datos</div>
                    <div class="value">
                        <xsl:value-of select="main/package/@fuente"/>
                    </div>

                    <div class="label">Creado Por</div>
                    <div class="value">
                        <xsl:value-of select="main/package/autor"/>
                    </div>

                    <div class="label">Fecha de Creación</div>
                    <div class="value">
                        <xsl:value-of select="main/package/fecha"/>
                    </div>

                    <div class="label">Descripción</div>
                    <div class="value">
                        <xsl:value-of select="main/package/descripcion"/>
                    </div>

                </div>

                <div class="hist-title">Historial de Modificaciones</div>

                <div class="hist-header">
                    <div>Fecha</div>
                    <div>Autor</div>
                    <div>Incidente</div>
                    <div>Descripción</div>
                </div>

                <div class="historial">

                    <xsl:for-each select="main/package/historial/modificacion">

                        <div class="hist-row">

                            <div class="col">
                                <xsl:value-of select="@fecha"/>
                            </div>

                            <div class="col">
                                <xsl:value-of select="@autor"/>
                            </div>

                            <div class="col">
                                <xsl:value-of select="@inc"/>
                            </div>

                            <div class="col descripcion">
                                <pre><xsl:value-of select="."/></pre>
                            </div>

                        </div>

                    </xsl:for-each>

                </div>

            </div>

        </div>

    </xsl:if>

    <!-- METODOS -->
    <xsl:for-each select="main/methods/procedure">

        <div class="card" id="met_{position()}">

            <div class="card-header"
                 onclick="toggleSection('met_{position()}')">

                Método:
                <xsl:value-of select="unidad"/>

            </div>

            <div class="card-body met_{position()}" style="display:none;">

                <div class="row">

                    <div class="label">Fuente de Datos</div>
                    <div class="value">
                        <xsl:value-of select="@fuente"/>
                    </div>

                    <div class="label">Creado Por</div>
                    <div class="value">
                        <xsl:value-of select="autor"/>
                    </div>

                    <div class="label">Fecha de Creación</div>
                    <div class="value">
                        <xsl:value-of select="fecha"/>
                    </div>

                    <div class="label">Descripción</div>
                    <div class="value">
                        <xsl:value-of select="descripcion"/>
                    </div>

                </div>

                <!-- RETORNO -->
                <xsl:if test="normalize-space(retorno/@nombre) != ''">

                    <div class="hist-title">Retorno</div>

                    <div class="retorno-header">
                        <div>Retorno</div>
                        <div>Tipo</div>
                        <div>Descripción</div>
                    </div>

                    <div class="retorno-row">

                        <div class="col">
                            <xsl:value-of select="retorno/@nombre"/>
                        </div>

                        <div class="col">
                            <xsl:value-of select="retorno/@tipo"/>
                        </div>

                        <div class="col descripcion">
                            <pre><xsl:value-of select="retorno"/></pre>
                        </div>

                    </div>

                </xsl:if>

                <!-- PARAMETROS -->
                <xsl:if test="parametros/param">

                    <div class="hist-title">Parámetros</div>

                    <div class="param-header">
                        <div>Parámetro</div>
                        <div>Tipo</div>
                        <div>In</div>
                        <div>Out</div>
                        <div>Default</div>
                        <div>Descripción</div>
                    </div>

                    <div class="historial">

                        <xsl:for-each select="parametros/param">

                            <div class="param-row">

                                <div class="col">
                                    <xsl:value-of select="@nombre"/>
                                </div>

                                <div class="col">
                                    <xsl:value-of select="@tipo"/>
                                </div>

                                <xsl:choose>
                                    <xsl:when test="@direccion = 'IN' or @direccion = 'In' or @direccion = 'in'">
                                        <div class="col">X</div>
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <div class="col"></div>
                                    </xsl:otherwise>
                                </xsl:choose>

                                <xsl:choose>
                                    <xsl:when test="@direccion = 'OUT' or @direccion = 'Out' or @direccion = 'out'">
                                        <div class="col">X</div>
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <div class="col"></div>
                                    </xsl:otherwise>
                                </xsl:choose>

                                <xsl:choose>
                                    <xsl:when test="normalize-space(@default) != ''">
                                        <div class="col">
                                            <xsl:value-of select="@default"/>
                                        </div>
                                    </xsl:when>
                                    <xsl:otherwise>
                                        <div class="col"></div>
                                    </xsl:otherwise>
                                </xsl:choose>

                                <div class="col descripcion">
                                    <pre><xsl:value-of select="."/></pre>
                                </div>

                            </div>

                        </xsl:for-each>

                    </div>

                </xsl:if>

                <!-- COMENTARIOS -->
                <xsl:if test="comentarios/com">

                    <div class="hist-title">Comentarios Método</div>

                    <div class="historial comentarios-metodo">

                        <xsl:for-each select="comentarios/com">

                            <div class="comentario-row">

                                <div class="comentario-col">
                                    <pre><xsl:value-of select="."/></pre>
                                </div>

                            </div>
                        </xsl:for-each>
                    </div>
                </xsl:if>

                <!-- HISTORIAL -->
                <div class="hist-title">Historial de Modificaciones</div>

                <div class="hist-header">
                    <div>Fecha</div>
                    <div>Autor</div>
                    <div>Incidente</div>
                    <div>Descripción</div>
                </div>

                <div class="historial">
                    <xsl:for-each select="historial/modificacion">
                        <div class="hist-row">
                            <div class="col">
                                <xsl:value-of select="@fecha"/>
                            </div>

                            <div class="col">
                                <xsl:value-of select="@autor"/>
                            </div>

                            <div class="col">
                                <xsl:value-of select="@inc"/>
                            </div>

                            <div class="col descripcion">
                                <pre><xsl:value-of select="."/></pre>
                            </div>
                        </div>
                    </xsl:for-each>

                </div>

            </div>

        </div>

    </xsl:for-each>
</div>
</body>
</html>
</xsl:template>
</xsl:stylesheet>