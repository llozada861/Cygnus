#
#  Propiedad intelectual de Open Systems (c).
#
#  Schell       :    CopyFileXSLTemplate.sh
#
#  Descripcion  :    Shell que se encarga de copiar los archivos que contiene 
#                    los datos de las platillas XSL en el directorio de trabajo 
#                    del servidor donde se encuentra la base de datos
#
#  Autor        :    José Alexander Samboni
#  Fecha        :    27-Jul-2006
#
#  Parametros   :    
#      $1          -- String de conexion (usuario/Password@string_conexion)
#  Retorna      :
#
#  Historia modificaciones
#
#  Fecha       Autor    Descripcion
#  27-Jul-2006 asamboni SAO48717
#  Construccion.
#

#
#  Propiedad intelectual de Open Systems (c).
#
#  Procedimiento:    GetWorkDirectory
#  Descripcion  :    Obtiene el nombre del directorio de trabajo
#                    
#  Fecha       Autor    Descripcion
#  27-Jul-2006 asamboni SAO48717
#  Construccion.
#
GetWorkDirectory ()
{
    sbDirectory=`sqlplus -s <<!
    $sbStriCone
    set timing off
    set feed off
    set heading off
    set serverout on
    DECLARE
        sbErrorMessage varchar2(2000);
	nuErrorCode number;
    BEGIN
	dbms_output.put_line('Direcotiorio='||GE_BOFileManagerXSLTemp.fsbGetDirectory);
    EXCEPTION
    when ex.CONTROLLED_ERROR  then
	Errors.getError(nuErrorCode, sbErrorMessage);
	dbms_output.put_line('Error=-1=['||nuErrorCode||':'||substr(sbErrorMessage,1,200)||']'); 

    when OTHERS then
	dbms_output.put_line('Error=-1=['||sqlcode||':'||substr(sqlerrm,1,200)||']'); 
    END;
/
!`
    sbError=$sbDirectory
    sbDirectory=`echo $sbDirectory|awk -F= '{ printf "%s", $2 }'`
    #echo "Directorio: ($sbDirectory)"
    if [ $sbDirectory = -1 ]
    then
        sbError=`echo $sbError|awk -F= '{ printf "%s", $3 }'`
	echo "ERROR : Error obteniendo directorio de trabajo:$sbError"
	exit -1
    fi
}

#
#  Propiedad intelectual de Open Systems (c).
#
#  Procedimiento:    GetHostName
#  Descripcion  :    Obtiene el nombre del servidor donde esta la base de datos
#                    
#  Fecha       Autor    Descripcion
#  27-Jul-2006 asamboni SAO48717
#  Construccion.
#
GetHostName ()
{
    sbHostName=`sqlplus -s <<!
    $sbStriCone
    set timing off
    set feed off
    set heading off
    set serverout on
    DECLARE
        sbErrorMessage varchar2(2000);
	nuErrorCode number;
    BEGIN
	dbms_output.put_line('Servidor='||GE_BOFileManagerXSLTemp.fsbGetHostName);
    EXCEPTION
    when ex.CONTROLLED_ERROR  then
	Errors.getError(nuErrorCode, sbErrorMessage);
	dbms_output.put_line('Error=-1=['||nuErrorCode||':'||substr(sbErrorMessage,1,200)||']'); 

    when OTHERS then
	dbms_output.put_line('Error=-1=['||sqlcode||':'||substr(sqlerrm,1,200)||']'); 
    END;
/
!`
    sbError=$sbHostName
    sbHostName=`echo $sbHostName|awk -F= '{ printf "%s", $2 }'`
    #echo "Servidor  : ($sbHostName)"
    if [ $sbDirectory = -1 ]
    then
        sbError=`echo $sbError|awk -F= '{ printf "%s", $3 }'`
	echo "ERROR : Error obteniendo el nombre del servidor:$sbError"
	exit -1
    fi
}

#
#  Propiedad intelectual de Open Systems (c).
#
#  Procedimiento:    CopyFile
#  Descripcion  :    Copia los archivos en el directorio de trabajo
#                    
#  Fecha       Autor    Descripcion
#  27-Jul-2006 asamboni SAO48717
#  Construccion.
#
CopyFile ()
{
sbLocalHostName=`hostname`

echo " "
echo " Servidor local de aplica: [$sbLocalHostName]"
echo " Servidor de Base Datos  : [$sbHostName]"
echo " Directorio de trabajo   : [$sbDirectory]"
echo " "

find . -name "*.txt" -type f |sort > lista

if [ $sbHostName = $sbLocalHostName ]
then
    #Comando local, si la base de datos esta en el mismo servidor donde se ejecuta el aplica
    sbCommandCopy=$sbDirectory
    # copia los archivos en el directorio XML_DIR
    for cada in `cat lista`
    do
	echo "Copiando  Archivo : $cada en $sbCommandCopy"
	cp $cada $sbCommandCopy
    done
else 
    #Comando remoto, si la base de datos esta en un servidor diferente de donde se ejecuta el aplica
    sbCommandCopy=$sbHostName:$sbDirectory
    # copia los archivos en el directorio XML_DIR
    for cada in `cat lista`
    do
	echo "Copiando  Archivo : $cada en $sbCommandCopy"
	rcp $cada $sbCommandCopy
	echo " "
    done
fi
rm lista
}

#
#  main()
#
# Inicialización de variables
sbStriCone=$1
# Obtiene el nombre del directorio de trabajo
GetWorkDirectory
# Obtiene el nombre del servidor donde se encuentra la base de datos
GetHostName
# Copia los archivos de donde se encuentran las plantillas al directorio de trabajo
CopyFile
