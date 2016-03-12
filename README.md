### 多技术服务端部署(一键脚本)
Support Tech
* VPN
* Shadowsocks
* obfsproxy
* Squid
* PAC
* 反馈[问题](https://plus.google.com/communities/101513261063592651175)
Debian/Ubuntu

```
wget https://raw.githubusercontent.com/squidproxy/squidproxy/master/other/Cross-technologydevelopment.sh

chmod +x Cross-technologydevelopment.sh

./Cross-technologydevelopment.sh

```

## SquidProxy V2.0.0.13##


* SquidProxy [下载](https://github.com/squidproxy/squidproxy/releases/download/V2.0.0.13/Squidproxy.exe)
* 修正了退出后代理无法恢复的bug
* 更新obfs4混淆部署 [Obfsproxy](https://github.com/squidproxy/obfs4)
* 对Squid流量做混淆加密处理,降低干扰和识别率,提高Squid服务器的稳定性

##特性##

- 支持Squid技术
- 多服务器管理,支持添加、删除、编辑Squid服务器
- 自动版本更新功能,轻松获取最新技术
- 支持智能和全局,无缝切换
- 基于Framework3.5框架开发
- PAC模式部署,安装apache2服务到服务器,确保Squid服务和Apache2为同一服务器!

## Server

### Install

Debian/Ubuntu

```
apt-get update 
apt-get -y install squid3

```

### Prepare execution folders 
创建缓存文件

```
mkdir /var/log/squid
mkdir /var/cache/squid
mkdir /var/spool/squid
chown -cR proxy /var/log/squid
chown -cR proxy /var/cache/squid
chown -cR proxy /var/spool/squid

squid3 -z

```

### Build configuration file (Debian)
创建配置文件给Debian系统,默认端口为25

```
rm -fr /etc/squid3/squid.conf
wget --no-check-certificate -O /etc/squid3/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/D-squidconf.conf

```

### Build configuration file (Ubuntu)
创建配置文件给Ubuntu系统,默认端口为25

```
rm -fr /etc/squid3/squid.conf
wget --no-check-certificate -O /etc/squid3/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/U-squidconf.conf

```

### Start 
启动

```
squid3 -v                       #查询squid版本
service squid3 restart          #重启squid服务
service squid3 status           #查看squid服务运行状态   
netstat -lntp                   #查询25端口是否启动
nano /var/log/squid3/access.log #查看squid访问日志文件

```
 
### Deployment PAC Service 

### Install Apache
部署Apache服务,创建PAC文件
```
sudo aptitude install apache2 apache2-doc
wget --no-check-certificate -O /var/www/html/Squidproxy.pac https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/Squidproxy.pac
nano /var/www/html/Squidproxy.pac

replace string VPS-IP with your server ip
将VPS-IP换成你的服务器IP 
```

### Auto development pac server
自动部署PAC技术
```
wget --no-check-certificate  https://raw.githubusercontent.com/squidproxy/squidproxy/master/PAC/autodevelopmentPAC.sh
chmod +x autodevelopmentPAC.sh
./autodevelopmentPAC.sh
 
```


### Test PAC Address
测试PAC地址
```
 If your server IP is 127.0.0.1,then your PAC address is http://127.0.0.1/Squidproxy.pac
 
 Smart mode (PAC) address is http://127.0.0.1/Squidproxy.pac

 global mode :The proxt Server address is 127.0.0.1 ,port 25
 
 Now,setting up PAC address for your Internet option 
 
```

### Install

centos

```
yum -y install squid wget
wget --no-check-certificate -O /etc/squid/squid.conf https://raw.githubusercontent.com/squidproxy/squidproxy/master/Squidconf/U-squidconf.conf

```

### Prepare execution folders 
创建缓存文件
```
mkdir -p /var/cache/squid
chmod -R 777 /var/cache/squid
squid -z

```

### Create firewall rules
创建防火墙规则
```
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

```
 
### Start squid service and start automatically during the system startup
重启服务和随系统启动
```
service squid restart
systemctl enable squid.service

```
### Install obfsproxy
obfs4对squid流量进行混淆加密
* obfsproxy [obfsproxy](https://github.com/squidproxy/obfs4)
* Client [obfsproxy](https://www.torproject.org/projects/torbrowser.html.en#downloads)

###Note
以上部署测试通过的服务器版本
```
CentOS Linux release 7.1.1503 (Core)
Debian 8 x64 (jessie)
Debian 7 x64 (jessie)
Ubuntu 14.02 x64

```

## Other Communities

* G+ [谷歌社区](https://plus.google.com/communities/101513261063592651175)
* Twitter: [推特](https://twitter.com/squidgfw)

###Acknowledgement 
特别鸣谢如下项目和作者,SquidProxy使用的技术全部来自如下项目,算是一个集大成者.


* Squid项目[squidcache](http://www.squid-cache.org) 他们发明的服务器缓存代理技术,使得我们有机会推广此技术服务大众 
* TOR项目 [TOR](https://www.torproject.org/)他们致力于研究匿名技术,squidProxy的服务端混淆技术就采用洋葱的[obfs4](https://github.com/squidproxy/obfs4)
* Flora PAC项目[Flora PAC](https://github.com/usufu/Flora_Pac/)高效的PAC函数,使得浏览器的智能识别得到提高
* 参照了clowwindy作品的部分思想和技术,其他项目就不一一列出.
* 这是一个业余程序员的作品,即便我们追求完美,纰漏瑕疵亦无可避免,欢迎到g+社区指正!


## Theory

数据缓存技术
在一个若干人共同使用的"宿舍"(Squid服务器),A同学(相当于A浏览器或A设备) 从"图书馆"(Youtube等网站)借来的"书籍"(网站等视频资源).
B同学(相当于B浏览器或B设备)不用在再跑去图书馆(远程网站)借这本书,直接在他们的宿舍(Squid服务器)阅读浏览借来的书籍即可.Squid会将这些视频数据保存在本地,供其他
用户调用。
这个从本地获取视频的效率，远比反复从图书馆来的效率更高! 这个就是Squid技术的原理!


## LICENSE

Copyright (C) 2016 Dave feng

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.