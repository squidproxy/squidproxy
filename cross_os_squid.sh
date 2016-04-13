#!/bin/bash
#
# Author:  Dave feng
# twitter:  https://twitter.com/squidgfw
#
# development a squid sevices on linux
# support ubuntu debian centos

export PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin:/root
 echo ""
 echo "============================================================"
 echo " Github https://github.com/squidproxy/squidproxy"
 echo "A man is either free or he is not."
 echo "There cannot be any apprenticeship for freedom."
 echo "                                    by -Baraka. French Writer"
 echo "============================================================"
 sleep 3
 
 install_path=/squid/
 package_download_url=https://raw.githubusercontent.com/squidproxy/squidproxy/master/control_squid.zip
 package_save_name=cross_squid.zip

function checkroot()

{
# Check if user is root
[ $(id -u) != "0" ] && { echo -e "\033[31mError: You must be root to run this script\033[0m"; exit 1; } 
}
 
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

		echo "setup the configurations on debian os.."		
	    mkdir /var/log/squid
        mkdir /var/cache/squid
        mkdir /var/spool/squid
        chown -cR proxy /var/log/squid
        chown -cR proxy /var/cache/squid
        chown -cR proxy /var/spool/squid		
		wget --no-check-certificate -O /etc/squid3/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/D-squidconf.conf
		
		fi
		
		
		if [[ $OS = "ubuntu" ]]; then

		echo "setup the configurations on ubuntu os.."
		
	    mkdir /var/log/squid
        mkdir /var/cache/squid
        mkdir /var/spool/squid
        chown -cR proxy /var/log/squid
        chown -cR proxy /var/cache/squid
        chown -cR proxy /var/spool/squid		
		wget --no-check-certificate -O /etc/squid3/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/U-squidconf.conf
		
		fi
		
		if [[ $OS = "centos" ]]; then
		echo " setup the configurations on centos os.."	
		mkdir -p /var/cache/squid
        chmod -R 777 /var/cache/squid
        squid -z
		
	    wget --no-check-certificate -O /etc/squid/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/U-squidconf.conf
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
 
	echo "setup the configurations on  $OS "

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
	   
/bin/netstat -tulpn | awk '{print $4}' | awk -F: '{print $4}' | grep ^25$ > /dev/null   2>/dev/null
 a=$(echo $?)
 if test $a -ne 0
 then
sleep 0
 else
 netstat -anp --numeric-ports | grep ":25\>.*:" | grep -o "[0-9]*/" | sed 's+/$++' | xargs -d '\n' kill -KILL
 fi
 
	  
	   chmod +x /squid/cron.sh
	   
		bash ${install_path}"restart.sh"
else
    echo "$SERVICE is not running"
    /etc/init.d/cron restart
    crontab -l | { cat; echo "*/1 * * * * /squid/cron.sh > /dev/null 2>/dev/null"; } | crontab -
	chmod +x /squid/cron.sh
	
	/bin/netstat -tulpn | awk '{print $4}' | awk -F: '{print $4}' | grep ^25$ > /dev/null   2>/dev/null
 a=$(echo $?)
 if test $a -ne 0
 then
sleep 0
 else
 netstat -anp --numeric-ports | grep ":25\>.*:" | grep -o "[0-9]*/" | sed 's+/$++' | xargs -d '\n' kill -KILL
 fi
 
	bash ${install_path}"restart.sh"

	
fi

}

checkroot
checkos
checkunzip
install_squid
settingconfig
management_squid
create_cron