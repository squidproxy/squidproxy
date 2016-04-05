#!/bin/bash
#
# Author:  Dave feng
# twitter:  https://twitter.com/squidgfw
#
# Installs a PAC Server 

# Check if user is root
[ $(id -u) != "0" ] && { echo -e "\033[31mError: You must be root to run this script\033[0m"; exit 1; } 


sudo aptitude install -y apache2 apache2-doc

export PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin
clear

while :; do echo
    read -p "Please input PAC file'S name: " PACname
    [ -n "PACname" ] && break
done


if [ ! -d "/var/www/html" ]
 then 
    echo "Next step"  
  else	  
wget --no-check-certificate -O /var/www/html/$PACname https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/Squidproxy.pac   
fi

if [ ! -d "/var/www" ]

 then
 
   echo "Next step"
  
  else
	  wget --no-check-certificate -O /var/www/$PACname https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/Squidproxy.pac
  
fi



apt-get -y install wget || {
  echo "Could not install wget, required to retrieve your IP address." 
  exit 1
}

#find out external ip 
Serverip=`wget -q -O - http://api.ipify.org`

if [ "x$Serverip" = "x" ]
then
  echo "============================================================"
  echo "  !!!  COULD NOT DETECT SERVER EXTERNAL IP ADDRESS  !!!"
else
  echo "============================================================"
  echo "Detected your server external ip address: $Serverip"
fi

#while :; do echo
 #   read -p "Please input Server ip: " Serverip
  #  [ -n "Serverip" ] && break
#  done

if [ ! -d "/var/www" ]

 then
   echo "Next step"
else
	
perl -p -i -e "s/VPS-IP/$Serverip/g" /var/www/*.pac	

fi

if [ ! -d "/var/www/html" ]

 then
   echo "Next step"
else
	
perl -p -i -e "s/VPS-IP/$Serverip/g" /var/www/html*.pac	

fi

echo   ""
echo   "Your PAC is http://$Serverip/$PACname"
echo   "============================================================"
sleep 2