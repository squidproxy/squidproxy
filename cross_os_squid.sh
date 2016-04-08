#!/bin/bash
#
# Author:  Dave feng
# twitter:  https://twitter.com/squidgfw
#
# development a squid sevices on linux
# support ubuntu debian centos


# Check if user is root
[ $(id -u) != "0" ] && { echo -e "\033[31mError: You must be root to run this script\033[0m"; exit 1; } 

# Begin 
# development VPN sevice on debian OS 
export PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin:/root
 echo ""
 echo "================================================================================================="
 echo " Github https://github.com/squidproxy/squidproxy"
 echo ""
 echo "================================================================================================="
 sleep 3
 
 install_path=/squid/
 package_download_url=https://raw.githubusercontent.com/squidproxy/squidproxy/master/control_squid.zip
 package_save_name=cross_squid.zip


 
 function checkos(){
    if [[ -f /etc/redhat-release ]];then
        OS=centos
    elif [[ ! -z "`cat /etc/issue | grep bian`" ]];then
        OS=debian
    elif [[ ! -z "`cat /etc/issue | grep Ubuntu`" ]];then
        OS=ubuntu
    else
        echo "Unsupported operating systems!"
        exit 1
    fi
	echo $OS
}

function checkunzip(){

if [ $(dpkg-query -W -f='${Status}' unzip 2>/dev/null | grep -c "ok installed") -eq 0 ];
then

 echo "======================================================"
 echo "start installing unzip package for you!"

 
 	if [[ $OS = "ubuntu" ]]; then
			echo " Install  ubuntu unzip ..."
			apt-get -y install unzip
		fi
		
		if [[ $OS = "debian" ]]; then
			echo " Install  debian unzip ..."
			apt-get -y install unzip
		fi
		
		if [[ $OS = "centos" ]]; then
			echo " Install  centos unzip ..."
			yum install -y unzip
		fi


	echo $result
	
else

echo "already installed unzip "

fi
	
}


function install_squid()

{
	
	if [[ $OS = "ubuntu" ]]; then
			echo " Install  ubuntu squid ..."
			apt-get -y install squid3
		fi
		
		if [[ $OS = "debian" ]]; then
			echo " Install  debian squid ..."
			apt-get -y install squid3
		fi
		
		if [[ $OS = "centos" ]]; then
			echo " Install  centos squid ..."
			yum install -y squid
		fi
		
 echo "start installing squid package for you!"
 
}


function settingconfig()

{

	if [[ $OS = "debian" ]]; then

		echo "setting configurate on debian os.."
		
	   mkdir /var/log/squid
      mkdir /var/cache/squid
      mkdir /var/spool/squid
      chown -cR proxy /var/log/squid
      chown -cR proxy /var/cache/squid
      chown -cR proxy /var/spool/squid		
		wget --no-check-certificate -O /etc/squid3/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/D-squidconf.conf
		
		fi
		
		
		if [[ $OS = "ubuntu" ]]; then

		echo "setting configurate on ubuntu os.."
		
	   mkdir /var/log/squid
      mkdir /var/cache/squid
      mkdir /var/spool/squid
      chown -cR proxy /var/log/squid
      chown -cR proxy /var/cache/squid
      chown -cR proxy /var/spool/squid		
		wget --no-check-certificate -O /etc/squid3/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/U-squidconf.conf
		
		fi
		
		if [[ $OS = "centos" ]]; then
		
		mkdir -p /var/cache/squid
      chmod -R 777 /var/cache/squid
      squid -z
 
      iptables -t nat -F
      iptables -t nat -X
      iptables -t nat -P PREROUTING ACCEPT
      iptables -t nat -P POSTROUTING ACCEPT
      iptables -t nat -P OUTPUT ACCEPT
      iptables -t mangle -F
      iptables -t mangle -X
      iptables -t mangle -P PREROUTING ACCEPT
      iptables -t mangle -P INPUT ACCEPT
      iptables -t mangle -P FORWARD ACCEPT
      iptables -t mangle -P OUTPUT ACCEPT
      iptables -t mangle -P POSTROUTING ACCEPT
      iptables -F
      iptables -X
      iptables -P FORWARD ACCEPT
      iptables -P INPUT ACCEPT
      iptables -P OUTPUT ACCEPT
      iptables -t raw -F
      iptables -t raw -X
      iptables -t raw -P PREROUTING ACCEPT
      iptables -t raw -P OUTPUT ACCEPT	  
      service iptables save
 
 fi
 	

}

 function management_squid()
 
 {
	rm -f $package_save_name
	echo "Download software..."
	if ! wget --no-check-certificate -O $package_save_name $package_download_url ; then
		echo "Download software failed!"
		exit 1
	fi

	if [[ ! -d "$install_path" ]]; then
		mkdir "$install_path"
		else
		echo "Update Software..."
	fi
	
	unzip -o $package_save_name  -d $install_path
	
	bash ${install_path}"restart.sh"

}


function create_cron()

{
#checking to see if a cron service is running 

SERVICE=cron;

if ps ax | grep -v grep | grep $SERVICE > /dev/null
then
    echo "$SERVICE service running, everything is fine"
	   crontab -r
	   crontab -l | { cat; echo "*/1 * * * * /squid/cron.sh > /dev/null 2>/dev/null"; } | crontab -
	   kill -9 $(lsof -i:25 -t) 2> /dev/null
	   chmod +x /squid/cron.sh
	   bash /squid/restart.sh
else
    echo "$SERVICE is not running"
    /etc/init.d/cron restart
	sleep 3
    crontab -l | { cat; echo "*/1 * * * * /squid/cron.sh > /dev/null 2>/dev/null"; } | crontab -
	chmod +x /squid/cron.sh
	kill -9 $(lsof -i:25 -t) 2> /dev/null
	bash /squid/restart.sh

	
fi

}

#dir=$(dirname $(readlink -f $file))
#echo $dir

checkos
checkunzip
install_squid
settingconfig
management_squid
create_cron