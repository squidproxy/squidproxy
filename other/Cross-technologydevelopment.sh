#!/bin/bash
#
# Author:  Dave feng
# twitter:  https://twitter.com/squidgfw
#
# Installs a PAC Server 

# Check if user is root
[ $(id -u) != "0" ] && { echo -e "\033[31mError: You must be root to run this script\033[0m"; exit 1; } 

# Begin 
# development VPN sevice on debian OS 
export PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin:/root
 echo ""
 echo "================================================================================================="
 echo " Squidproxy Project will teach you how to development  a proxy server "
 echo " The Shadowsocks Squid obfs4 and VPN services will have been installed on your linux OS "
 echo " Suuport OS : Debian Ubuntu"
 echo " Version V1.0.0.3 "
 echo " Github https://github.com/squidproxy/squidproxy"
 echo ""
 echo "================================================================================================="
 sleep 3
 
 apt-get update
 
 if [ $(dpkg-query -W -f='${Status}' pptpd 2>/dev/null | grep -c "ok installed") -eq 0 ];
then
 
  wget --no-check-certificate https://raw.github.com/viljoviitanen/setup-simple-pptp-vpn/master/setup.sh

chmod +x setup.sh

./setup.sh

cat /etc/ppp/chap-secrets

 clear
 echo "======================================================"
 echo "development VPN sevices compeleted ,congratulations!"
 echo "======================================================"
 sleep 3
  
  else
	  
     clear
 echo "===================================================="
 echo "your  VPN  sevice already exists ,congratulations!"
 echo "====================================================="
 sleep 3
 

fi

# development VPN sevice on debian OS

# END

# Begin 
# development Shadowsocks sevice on debian OS

 if [ $(dpkg-query -W -f='${Status}' python-pip 2>/dev/null | grep -c "ok installed") -eq 0 ];
then
 
apt-get -y install python-pip

else

 echo "======================================================"
 echo "python-pip has installed ,congratulations!"

 sleep 3
fi


pip install shadowsocks


if lsof -Pi :443 -sTCP:LISTEN -t >/dev/null ; then
 echo "=============================================="
 echo "Shadowsocks 443 port aleady running !"

 sleep 3
 
else
 nohup ssserver -p 443 -k Wi2Iaqui -m rc4 &
 clear
 echo "==========================================================="
 echo "implement ssserver command success,443 port already running 
 echo "==========================================================="
 sleep 2
 

fi

if lsof -Pi :7700 -sTCP:LISTEN -t >/dev/null ; then


 echo "The Shadowsocks 7700 port aleady running !"

 sleep 3

else
 nohup ssserver -p 7700 -k Wi2Iaqui -m rc4 &

 clear
 echo "============================================================="
 echo "implement ssserver command success,7700 port already running "
 echo "============================================================="
 sleep 2
 
fi

if lsof -Pi :8800 -sTCP:LISTEN -t >/dev/null ; then


 echo "The Shadowsocks 8800 port aleady running !"

 sleep 3

else
 nohup ssserver -p 8800 -k Wi2Iaqui -m rc4 &

 clear
 echo "============================================================"
 echo "implement ssserver command success,8800 port already running "
 echo "============================================================"
 sleep 3
 
fi

if lsof -Pi :9911 -sTCP:LISTEN -t >/dev/null ; then


 echo "The Shadowsocks 9911 port aleady running !"

 sleep 3
		
else
	
 nohup ssserver -p 9911 -k Wi2Iaqui -m rc4 &
 
 clear
 echo "============================================================"
 echo "implement ssserver command success,9911 port already running "
 echo "============================================================"
 sleep 2
 
fi



 echo "development Shadowsocks already compeleted ,congratulations!"
 echo "============================================================"
 sleep 3

# development Shadowsocks sevice on debian OS
# END


# Begin 
# development squid sevice on debian OS


# Install squid3 and aptitude sudo package 

 if [ $(dpkg-query -W -f='${Status}' squid3 2>/dev/null | grep -c "ok installed") -eq 0 ];
	
then
	
	
apt-get -y install squid3 aptitude sudo
mkdir /var/log/squid
mkdir /var/cache/squid
mkdir /var/spool/squid
chown -cR proxy /var/log/squid
chown -cR proxy /var/cache/squid
chown -cR proxy /var/spool/squid
squid3 -z

rm -fr /etc/squid3/squid.conf
wget --no-check-certificate -O /etc/squid3/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/D-squidconf.conf
service squid3 restart

 clear
 echo "=========================================================="
 echo "development squid sevice compeleted ,congratulations!"
 echo "========================================================="
 sleep 3
 
# development squid sevice on debian OS
# END

else
	   clear
 echo "==================================================="
 echo "your squid sevice already exists ,congratulations!"
 echo "==================================================="
 sleep 3


fi
	
if lsof -Pi :25 -sTCP:LISTEN -t >/dev/null ; then
 

  clear
 echo "=============================================="
 echo  "The squid 25 port aleady running !good luck "
 echo "=============================================="
 sleep 3
 
 else
	  clear
 echo "=========================================================="
 echo "implement squid start command success,25 port running now"
 echo "=========================================================="
 service squid3 restart 
 sleep 2 
 
 fi
 
clear 
 echo "=============================================="
 echo  "Check port 1723 status"
 echo "=============================================="
 sleep 3
 
 if lsof -Pi :1723 -sTCP:LISTEN -t >/dev/null ; then
 

 echo "=============================================="
 echo  "The vpn 1723 port aleady running !good luck"
 echo "=============================================="
 sleep 3
 
 else

 service pptpd restart 	 
 echo "==========================================================="
 echo "implement VPN start command success,1723 port running now"
 echo "==========================================================="	 
 
 fi
 

clear

while :; do echo
    read -p "Please input PAC file name: " PACname
    [ -n "PACname" ] && break
done

sudo aptitude install  apache2 apache2-doc


if [ ! -d "/var/www" ]

 then
 
  echo "please remove your apache2 sevice and try again!!" 
  
  else
 wget --no-check-certificate -O /var/www/$PACname https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/Squidproxy.pac    
	
fi

if [ ! -d "/var/www/html" ]

 then
 
  echo "please remove your apache2 sevice and try again!!" 
  
  else
 wget --no-check-certificate -O /var/www/html/$PACname https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/Squidproxy.pac    
	
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

clear
 echo "=============================================="
 echo "begin Optimizing Server performance now"

 sleep 3
# Optimizing Server performance
cat >> /etc/sysctl.d/local.conf << EOF
# max open files
fs.file-max = 51200
# max read buffer
net.core.rmem_max = 67108864
# max write buffer
net.core.wmem_max = 67108864
# default read buffer
net.core.rmem_default = 65536
# default write buffer
net.core.wmem_default = 65536
# max processor input queue
net.core.netdev_max_backlog = 4096
# max backlog
net.core.somaxconn = 4096

# resist SYN flood attacks
net.ipv4.tcp_syncookies = 1
# reuse timewait sockets when safe
net.ipv4.tcp_tw_reuse = 1
# turn off fast timewait sockets recycling
net.ipv4.tcp_tw_recycle = 0
# short FIN timeout
net.ipv4.tcp_fin_timeout = 30
# short keepalive time
net.ipv4.tcp_keepalive_time = 1200
# outbound port range
net.ipv4.ip_local_port_range = 10000 65000
# max SYN backlog
net.ipv4.tcp_max_syn_backlog = 4096
# max timewait sockets held by system simultaneously
net.ipv4.tcp_max_tw_buckets = 5000
# turn on TCP Fast Open on both client and server side
net.ipv4.tcp_fastopen = 3
# TCP receive buffer
net.ipv4.tcp_rmem = 4096 87380 67108864
# TCP write buffer
net.ipv4.tcp_wmem = 4096 65536 67108864
# turn on path MTU discovery
net.ipv4.tcp_mtu_probing = 1

# for high-latency network
# net.ipv4.tcp_congestion_control = hybla

# for low-latency network, use cubic instead
 net.ipv4.tcp_congestion_control = cubic
EOF
sysctl --system
# Optimizing Server performance

clear
 echo ""
 echo "Optimizing Server performance compeleted ,congratulations!"
 echo "=============================================================="
 sleep 3

clear
 echo "=============================================="
 echo "Start developing obfsproxy services ......."
 echo "=============================================="
 sleep 3
 
# development obfsproxy 
apt-get -y install gcc python-pip python-dev
pip install obfsproxy

 clear 
 echo "=============================================="
 echo  "Check port 8087 obfs4 status "

 sleep 3
 
if lsof -Pi :8087 -sTCP:LISTEN -t >/dev/null ; then
  

 echo  "obfsproxy 8087 port running!good luck"

 sleep 3

else
	
/usr/local/bin/obfsproxy --data-dir=/tmp/scramblesuit-server scramblesuit --password=FANGBINXINGFUCKYOURMOTHERSASS444 --dest=127.0.0.1:25 server 0.0.0.0:8087 &
 


 echo "implement obfs4 start command success,8087 port running now"
 echo "============================================================="	 

 
fi


 echo "development obfsproxy already compeleted ,congratulations!"
 echo "============================================================"
 sleep 3
 
 
 clear 
 echo "========================================================="
 echo "start automatically during the system startup "
 echo "=========================================================="	
 sleep 3
 
# start automatically during the system startup (Debian)
cat >> /etc/rc.local  << EOF
(/usr/local/bin/obfsproxy --data-dir=/tmp/scramblesuit-server scramblesuit --password=FANGBINXINGFUCKYOURMOTHERSASS444 --dest=127.0.0.1:25 server 0.0.0.0:23333 >/dev/null 2>&1 &)
EOF

clear 
echo   "============================================================"
echo   ""
echo   "squid PAC address http://$Serverip/$PACname"
echo   "squid port 25"
echo   ""
echo   "Shadowsocks port 443,7700,8800,9911"
echo   "Shadowsocks password Wi2Iaqui"
echo   "Shadowsocks method rc4"
echo   ""
echo   "obfsproxy Bse32 FANGBINXINGFUCKYOURMOTHERSASS444"
echo   "obfsproxy listen port 8087"
echo   ""
echo   "Copyright (C) 2016 squidproxy project"
echo   "Write by Dave feng 3.12/2016"

echo   "============================================================"
read -p "Press any key to continue." var