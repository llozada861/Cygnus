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
    background:#7CD300;
}

.active-link{
    background:#007934;
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
    background:#007934;
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

    <img data-imagetype="DataUri" src="data:image/png;base64,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" width="101" height="44" title="EPM" alt="EPM" style="margin:auto; border:0px currentColor; display:block"/>

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