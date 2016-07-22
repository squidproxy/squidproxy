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

##### echo
function print_xxxx(){
    xXxX="#############################"
    echo
    echo "$xXxX$xXxX$xXxX$xXxX"
    echo
}

function install_lsof(){

    print_info "installing  lsof package on linux"
        	if [[ $OS = "ubuntu" ]]; then
			echo " Install  ubuntu lsof ..."
			apt-get -y install lsof
		fi
		if [[ $OS = "debian" ]]; then
			echo " Install  debian lsof ..."
			apt-get -y install lsof
		fi
		if [[ $OS = "centos" ]]; then
			echo " Install  centos lsof ..."
			yum -y install lsof
		fi

}


function print_warn(){
    echo -n -e '\033[41;37m'
    echo -n $1
    echo -e '\033[0m'
}


function die(){
    echo -e "\033[33mERROR: $1 \033[0m" > /dev/null 1>&2
    exit 1
}

function print_info(){
    echo -n -e '\e[1;36m'
    echo -n $1
    echo -e '\e[0m'
}


function check_package(){
      PORT=22
      lsof -i:$PORT > /dev/null 2>&1 && print_info "installed lsof"  || install_lsof

}

function install_squid()

{
	if [[ $OS = "ubuntu" ]]; then
			echo " Install  ubuntu squid ..."
			apt-get -y install squid3 && print_info "installing squid service"
		fi
		if [[ $OS = "debian" ]]; then
			echo " Install  debian squid ..."
			apt-get -y install squid3 && print_info "installing squid service"
		fi
		if [[ $OS = "centos" ]]; then
			echo " Install  centos squid ..."
			yum install -y squid && print_info "installing squid service"
		fi
}

function firewall_rules(){

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

}

function settingconfig()

{

	if [[ $OS = "debian" ]]; then

		echo "setup the configurations on debian o"
	    mkdir /var/log/squid
        mkdir /var/cache/squid
        mkdir /var/spool/squid
        chown -cR proxy /var/log/squid
        chown -cR proxy /var/cache/squid
        chown -cR proxy /var/spool/squid
		wget --no-check-certificate -O /etc/squid3/squid.conf https://git.io/vwMcb && print_info "fetch conf from github success"

	fi
		if [[ $OS = "ubuntu" ]]; then

		echo "setup the configurations on ubuntu os.."
	    mkdir /var/log/squid
        mkdir /var/cache/squid
        mkdir /var/spool/squid
        chown -cR proxy /var/log/squid
        chown -cR proxy /var/cache/squid
        chown -cR proxy /var/spool/squid
		wget --no-check-certificate -O /etc/squid3/squid.conf https://git.io/vwMcb && print_info "fetch conf from github success"
		fi

	    if [[ $OS = "centos" ]]; then
	    echo " setup the configurations on centos os.."
	    mkdir -p /var/cache/squid
            chmod -R 777 /var/cache/squid
            squid -z
	    wget --no-check-certificate -O /etc/squid/squid.conf https://git.io/vwMcb  && print_info "fetch conf from github"
	    print_info "finishing firewall rules"
            firewall_rules
            fi

}

function check_squidconf(){

 print_info "Check squid.conf"
 

	   if [[ $OS = "ubuntu" ]]; then
            PROT_NUM_debian=$(sed -n '3p' /etc/squid3/squid.conf)
           if [[ $PROT_NUM_debian = "http_port 25" ]]; then
		
                print_info "squid.conf correct"  

           else
              print_info "squid.conf Incorrect"  
           fi
		fi
		
		if [[ $OS = "debian" ]]; then
		PROT_NUM_debian=$(sed -n '3p' /etc/squid3/squid.conf)
             if [[ $PROT_NUM_debian = "http_port 25" ]]; then
		
           print_info "squid.conf correct"  

             else
               print_info "squid.conf Incorrect"  
             fi
		fi
		
		if [[ $OS = "centos" ]]; then
		    PROT_NUM_centos=$(sed -n '3p' /etc/squid/squid.conf)
            if [[ $PROT_NUM_centos = "http_port 25" ]]; then
		
               print_info "squid.conf correct"  

                    else
                    print_info "squid.conf Incorrect"  
            fi
		fi
		
		
}


function start_squid(){

	if [[ $OS = "ubuntu" ]]; then
		    print_info " start squid service..."
			service restart squid3 && print_info "restart squid service success on $OS"
		fi
		if [[ $OS = "debian" ]]; then
			print_info " start squid service..."
			service restart squid3 && print_info "restart squid service success on $OS"
		fi
		if [[ $OS = "centos" ]]; then
			print_info " start squid service..."
			systemctl restart squid && print_info "restart squid service success on $OS"
		fi


}

function check_port(){
print_info "check squid status........................."
lsof -i:25 && print_info "squid start ...success" || start_squid

}



function install_web(){



	if [[ $OS = "ubuntu" ]]; then
			apt-get -y install apache2 && print_info "installing apache2 service"
		fi
		if [[ $OS = "debian" ]]; then
			echo " Install  debian apache2 ..."
			apt-get -y install apache2 && print_info "installing apache2 service"
		fi
		if [[ $OS = "centos" ]]; then
			echo " Install  centos httpd ..."
			yum install -y httpd && print_info "installing httpd service"
		fi

}


function check_apache2(){

www="/var/www/"
html="/var/www/html/"

if [ ! -d "$html" ] && [ ! -d "$www" ];then

print_info "apache2 installing "

install_web

else

print_info "apache2 installed "

fi

}

function Start_http(){



	if [[ $OS = "ubuntu" ]]; then
			service apache2 restart && print_info "restart apache2  service success "
		fi
		if [[ $OS = "debian" ]]; then
                   service apache2 restart && print_info "restart apache2  service success "	
         	fi
		if [[ $OS = "centos" ]]; then
                  service httpd restart && print_info "restart httpd  service success "
		fi


}


function check_httpd_status(){

lsof -i:80 && print_info "check http service ok" || Start_http

}


function fetch_pac(){


www="/var/www/"
html="/var/www/html/"
pac=".pac"

if [ ! -d "$html" ];then 

     if [ ! -d "$www" ];then 
	 print_warn "please install http service"
	 else
     
     string_pac_path=$www
     print_info "current web path is $string_pac_path"
	 while :; do echo
    read -p "Please input pac  name: " PACname
    [ -n "PACname" ] && break
    done
    wget --no-check-certificate -O $string_pac_path$PACname$pac https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/Squidproxy.pac  && print_info "fetch pac rules from github"
	
	 fi

else

string_pac_path=$html

print_info "current web path is $string_pac_path"
while :; do echo
    read -p "Please input pac  name: " PACname
    [ -n "PACname" ] && break
done

 wget --no-check-certificate -O $string_pac_path$PACname$pac https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/Squidproxy.pac  && print_info "fetch pac rules  from github"
fi

if [ ! -f "$string_pac_path$PACname$pac" ];then

print_info "$string_pac_path$PACname$pac"

 print_warn "fetch pac rules failure" 

else

 print_info "fetch pac rules success"
  print_info "your pac path : $string_pac_path$PACname$pac"

fi

}


function serverspeeder(){

print_info "install serverspeeder ...."

wget -N --no-check-certificate https://raw.githubusercontent.com/91yun/serverspeeder/master/serverspeeder-all.sh && bash serverspeeder-all.sh


}





function replace_pac_string(){

#find out external ip 
Serverip=`wget -q -O - http://api.ipify.org`

print_info "$string_pac_path"
port=":25"
if [ "x$Serverip" = "x" ]
then
  echo "============================================================"
  echo "  !!!  COULD NOT DETECT SERVER EXTERNAL IP ADDRESS  !!!"
else
  echo "============================================================"
  echo "Detected your server external ip address: $Serverip"
fi

sed -i.bak "s/VPS-IP:25/$Serverip$port/g" "$string_pac_path$PACname$pac"

print_info "auto generate online pac address"

print_info "http://$Serverip/$PACname$pac"

print_info "squid port: 25"

print_info "Squid Server ip $Serverip"

lsof -i:25 > /dev/null 2>/dev/null && print_info "squid status ...ok" || print_warn "squid start ...failure" 

}

function help_ocservauto(){
    print_xxxx
    print_info "######################## Parameter Description ####################################"
    echo
    print_info " install ----------------------- Install squid  for Debian centos"
    echo
    print_info " gp   ---------------- auto generate online pac address "
    echo
    print_info " rs --------------- remove squid service on os "
    echo
    print_info " help or h --------------------- Show this description"
    print_xxxx
}

function remove_squid(){


yum remove squid -y
}

#Initialization step
action=$1
[  -z $1 ] && action=install
case "$action" in
install)
checkroot
checkos
check_package
install_squid
settingconfig
check_squidconf
check_port
check_apache2
check_httpd_status
fetch_pac
replace_pac_string
serverspeeder
    ;;
rs)
remove_squid
    ;;
gp)
fetch_pac
replace_pac_string
    ;;
help | h)
    clear
    help_ocservauto
    ;;
*)
    clear
    print_warn "Arguments error! [ ${action} ]"
    print_warn "Usage:  bash `basename $0` {gp|help|rs}"
    help_ocservauto
    ;;
esac
