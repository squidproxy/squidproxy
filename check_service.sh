#!/bin/bash
#
# Author:  Dave feng
# twitter:  https://twitter.com/squidgfw
#
# yongxiang SS server

# Check if user is root
[ $(id -u) != "0" ] && { echo -e "\033[31mError: You must be root to run this script\033[0m"; exit 1; } 
[ ! -f /etc/debian_version ] && die "Must be run on a Debian-based system."
# Begin 
# development VPN sevice on debian OS 
export PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin:/root

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
PORT=25
lsof -i:$PORT > /dev/null 2>&1 && print_info "installed lsof"  || apt-get -y install lsof
dpkg-query -W -f='${Status} ${Version}\n' squid3 > /dev/null 2>&1 && print_info "installed dpkg" || print_info "install dpkg" || apt-get -y install dpkg
}


function LOOP_START_SHADOWSOCKS()
{
i=3077;
while  [[ $i<3087 ]];do
echo $i
ssserver -p $i -k Wi2Iaqui -m rc4-md5  &
i=$((i+1))
done;
}


function check_status(){

for PORT in 25 1723 150

do
if  ! lsof -i:$PORT > /dev/null   2>/dev/null  
then

#    echo $PORT is free
if [ "$PORT" = "25" ]; then
              echo squid unrunning
              service squid3 restart > /dev/null 2>/dev/null
              fi
if [ "$PORT" = "150" ]; then
              echo fs unrunning
              /fs/restart.sh  > /dev/null 2>/dev/null
              fi
if [ "$PORT" = "1723" ]; then
              echo pptpd unrunning
              service pptpd restart > /dev/null 2>/dev/null
              fi

else
 #   echo $PORT is occupied

if [ "$PORT" = "25" ]

              then
              echo squid running
              fi
 if [ "$PORT" = "150" ]
              then
              echo fs running
              fi
 if [ "$PORT" = "1723" ]
              then
              echo pptpd running
              fi
fi

done;

}

#LOOP_START_SHADOWSOCKS
check_package
check_status

exit 0
