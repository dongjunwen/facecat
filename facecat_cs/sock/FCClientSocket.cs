/*捂脸猫FaceCat框架 v1.0
 1.创始人-矿洞程序员-上海宁米科技创始人-脉脉KOL-陶德 (微信号:suade1984);
 2.联合创始人-上海宁米科技创始人-袁立涛(微信号:wx627378127);
 3.联合创始人-肖添龙(微信号:xiaotianlong_luu);
 4.联合开发者-陈晓阳(微信号:chenxiaoyangzxy)，助理-朱炜(微信号:cnnic_zhu);
 5.该框架开源协议为BSD，欢迎对我们的创业活动进行各种支持，欢迎更多开发者加入。
 包含C/C++,Java,C#,iOS,MacOS,Linux六个版本的图形和通讯服务框架。
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace FaceCat {
    /// <summary>
    /// 客户端套接字连接
    /// </summary>
    public class FCClientSocket {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="proxyType">代理类型</param>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="proxyIp">代理IP</param>
        /// <param name="proxyPort">代理端口</param>
        /// <param name="proxyUserName">用户名</param>
        /// <param name="proxyUserPwd">密码</param>
        /// <param name="proxyDomain">域</param>
        /// <param name="timeout">超时</param>
        public FCClientSocket(int type, long proxyType, String ip, int port, String proxyIp, int proxyPort, String proxyUserName, String proxyUserPwd, String proxyDomain, int timeout) {
            m_blnProxyServerOk = true;
            m_proxyDomain = proxyDomain;
            m_proxyType = proxyType;
            m_ip = ip;
            m_port = port;
            m_proxyIp = proxyIp;
            m_proxyPort = proxyPort;
            m_proxyUserName = proxyUserName;
            m_proxyUserPwd = proxyUserPwd;
            m_timeout = timeout;
            m_type = type;
        }

        /// <summary>
        /// 代理服务是否OK
        /// </summary>
        private bool m_blnProxyServerOk;
        /// <summary>
        /// 是否连接
        /// </summary>
        private bool m_connected;
        /// <summary>
        /// 套接字ID
        /// </summary>
        public int m_hSocket;
        /// <summary>
        /// IP地址
        /// </summary>
        private String m_ip;
        /// <summary>
        /// 是否删除
        /// </summary>
        private bool m_isDeleted;
        /// <summary>
        /// 端口
        /// </summary>
        private int m_port;
        /// <summary>
        /// 域
        /// </summary>
        private String m_proxyDomain;
        /// <summary>
        /// 代理类型
        /// </summary>
        private long m_proxyType;
        /// <summary>
        /// 代理IP
        /// </summary>
        private String m_proxyIp;
        /// <summary>
        /// 代理端口
        /// </summary>
        private int m_proxyPort;
        /// <summary>
        /// 代理用户名
        /// </summary>
        private String m_proxyUserName;
        /// <summary>
        /// 代理密码
        /// </summary>
        private String m_proxyUserPwd;
        /// <summary>
        /// 套接字对象
        /// </summary>
        private Socket m_socket = null;
        /// <summary>
        /// 超时
        /// </summary>
        private int m_timeout;
        /// <summary>
        /// 类型
        /// </summary>
        private int m_type;
        /// <summary>
        /// UDP套接字对象
        /// </summary>
        private Socket m_udpSocket = null;

        /// <summary>
        /// 析构函数
        /// </summary>
        ~FCClientSocket() {
            delete();
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        /// <returns>是否关闭</returns>
        public int close() {
            int ret = -1;
            if (m_socket != null) {
                try {
                    m_socket.Close();
                    ret = 1;
                }
                catch (Exception ex) {
                    byte[] rmsg = new byte[1];
                    rmsg[0] = (byte)((char)2);
                    FCClientSockets.recvClientMsg(m_hSocket, m_hSocket, rmsg, 1);
                    FCClientSockets.writeClientLog(m_hSocket, m_hSocket, 2, "socket exit");
                    ret = -1;
                }
            }
            if (m_udpSocket != null) {
                try {
                    m_udpSocket.Close();
                    ret = 1;
                }
                catch (Exception ex) {
                    byte[] rmsg = new byte[1];
                    rmsg[0] = (byte)((char)2);
                    FCClientSockets.recvClientMsg(m_hSocket, m_hSocket, rmsg, 1);
                    FCClientSockets.writeClientLog(m_hSocket, m_hSocket, 2, "udp exit");
                    ret = -1;
                }
            }
            m_connected = false;
            return ret;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public ConnectStatus connect() {
            return connectStandard();
        }

        /// <summary>
        /// 标准连接
        /// </summary>
        /// <returns>状态</returns>
        private ConnectStatus connectStandard() {
            ConnectStatus status = ConnectStatus.CONNECT_SERVER_FAIL;
            IPAddress ip = IPAddress.Parse(m_ip);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                clientSocket.Connect(new IPEndPoint(ip, m_port));
                status = ConnectStatus.SUCCESS;
                m_connected = true;
                Thread tThread = new Thread(new ThreadStart(run));
                tThread.Start();
            }
            catch {
            }
            return status;
        }

        /// <summary>
        /// HTTP代理连接
        /// </summary>
        /// <returns>状态</returns>
        private ConnectStatus connectByHttp() {
            return ConnectStatus.SUCCESS;
        }

        /// <summary>
        /// Sock4代理连接
        /// </summary>
        /// <returns>状态</returns>
        private ConnectStatus connectBySock4() {
            return ConnectStatus.SUCCESS;
        }

        /// <summary>
        /// Sock5代理连接
        /// </summary>
        /// <returns>状态</returns>
        private ConnectStatus connectBySock5() {
            return ConnectStatus.SUCCESS;
        }

        /// <summary>
        /// 连接代理服务器
        /// </summary>
        /// <returns>状态</returns>
        private ConnectStatus connectProxyServer() {
            return ConnectStatus.SUCCESS;
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void delete() {
            if (!m_isDeleted) {
                close();
                m_connected = false;
                m_isDeleted = true;
            }
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void run() {
            byte[] str = null;
            bool get = false;
            int head = 0, pos = 0, strRemain = 0, bufferRemain = 0, index = 0, len = 0;
            int intSize = 4;
            byte[] headStr = new byte[4];
            int headSize = 4;
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);//定义要发送的计算机的地址
            EndPoint Remote = (EndPoint)(sender);//
            while (m_connected) {
                try {
                    byte[] buffer = new byte[10240];
                    if (m_type == 0) {
                        len = m_socket.Receive(buffer);
                    }
                    else if (m_type == 1) {
                        m_udpSocket.ReceiveFrom(buffer, ref Remote);
                    }
                    if (len == 0 || len == -1) {
                        byte[] rmsg = new byte[1];
                        rmsg[0] = (byte)((char)3);
                        FCClientSockets.recvClientMsg(m_hSocket, m_hSocket, rmsg, 1);
                        FCClientSockets.writeClientLog(m_hSocket, m_hSocket, 3, "socket error");
                        break;
                    }
                    index = 0;
                    while (index < len) {
                        int diffSize = 0;
                        if (!get) {
                            diffSize = intSize - headSize;
                            if (diffSize == 0) {
                                head = ((byte)(0xff & buffer[index]) | (byte)(0xff00 & (buffer[index + 1] << 8)) | (byte)(0xff0000 & (buffer[index + 2] << 16)) | (byte)(0xff000000 & (buffer[index + 3] << 24)));
                            }
                            else {
                                for (int i = 0; i < diffSize; i++) {
                                    headStr[headSize + i] = buffer[i];
                                }
                                head = ((byte)(0xff & headStr[0]) | (byte)(0xff00 & (headStr[1] << 8)) | (byte)(0xff0000 & (headStr[2] << 16)) | (byte)(0xff000000 & (headStr[3] << 24)));
                            }
                            if (str != null) {
                                str = null;
                            }
                            str = new byte[head];
                            if (diffSize > 0) {
                                for (int i = 0; i < headSize; i++) {
                                    str[i] = headStr[i];
                                }
                                pos += headSize;
                                headSize = intSize;
                            }
                        }
                        bufferRemain = len - index;
                        strRemain = head - pos;
                        get = strRemain > bufferRemain;
                        int remain = Math.Min(strRemain, bufferRemain);
                        Array.Copy(buffer, index, str, pos, remain);
                        pos += remain;
                        index += remain;
                        if (!get) {
                            FCClientSockets.recvClientMsg(m_hSocket, m_hSocket, str, head);
                            head = 0;
                            pos = 0;
                            if (len - index == 0 || len - index >= intSize) {
                                headSize = intSize;
                            }
                            else {
                                headSize = bufferRemain - strRemain;
                                for (int j = 0; j < headSize; j++) {
                                    headStr[j] = buffer[index + j];
                                }
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    break;
                }
            }
            byte[] rmsg2 = new byte[1];
            rmsg2[0] = (byte)((char)2);
            FCClientSockets.recvClientMsg(m_hSocket, m_hSocket, rmsg2, 1);
            FCClientSockets.writeClientLog(m_hSocket, m_hSocket, 2, "socket exit");
            m_connected = false;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        public int send(byte[] str, int len) {
            if (m_socket == null || !m_connected) {
                return -1;
            }
            return m_socket.Send(str);
        }

        /// <summary>
        /// 发送UDP数据
        /// </summary>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        public int sendTo(byte[] str, int len) {
            if (m_udpSocket == null || !m_connected) {
                return -1;
            }
            return m_udpSocket.Send(str);
        }
    }
}
