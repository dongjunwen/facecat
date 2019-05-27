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

namespace FaceCat {
    /// <summary>
    /// 客户端套接字管理
    /// </summary>
    public class FCClientSockets {
        /// <summary>
        /// 客户端套接字集合
        /// </summary>
        private static HashMap<int, FCClientSocket> m_clients = new HashMap<int, FCClientSocket>();

        /// <summary>
        /// 监听者
        /// </summary>
        private static FCSocketListener m_listener;

        /// <summary>
        /// 套接字ID
        /// </summary>
        private static int m_socketID;

        /// <summary>
        /// 关闭服务
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <returns>状态</returns>
        public static int close(int socketID) {
            int ret = -1;
            if (m_clients.containsKey(socketID)) {
                FCClientSocket client = m_clients.get(socketID);
                ret = client.close();
                m_clients.remove(socketID);
            }
            return ret;
        }

        /// <summary>
        /// 连接
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
        /// <returns>状态</returns>
        public static int connect(int type, int proxyType, String ip, int port, String proxyIp, int proxyPort, String proxyUserName, String proxyUserPwd, String proxyDomain, int timeout) {
            FCClientSocket client = new FCClientSocket(type, (long)proxyType, ip, port, proxyIp, proxyPort, proxyUserName, proxyUserPwd, proxyDomain, timeout);
            ConnectStatus ret = client.connect();
            if (ret != ConnectStatus.CONNECT_SERVER_FAIL) {
                m_socketID++;
                int socketID = m_socketID;
                client.m_hSocket = m_socketID;
                m_clients.put(socketID, client);
                return socketID;
            }
            else {
                client.delete();
                return -1;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="localSID">本地ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        public static void recvClientMsg(int socketID, int localSID, byte[] str, int len) {
            m_listener.callBack(socketID, localSID, str, len);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        public static int send(int socketID, byte[] str, int len) {
            int ret = -1;
            if (m_clients.containsKey(socketID)) {
                FCClientSocket client = m_clients.get(socketID);
                ret = client.send(str, len);
            }
            return ret;
        }

        /// <summary>
        /// 发送UDP数据
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        public static int sendTo(int socketID, byte[] str, int len) {
            int ret = -1;
            if (m_clients.containsKey(socketID)) {
                FCClientSocket client = m_clients.get(socketID);
                ret = client.sendTo(str, len);
            }
            return ret;
        }

        /// <summary>
        /// 设置监听者
        /// </summary>
        /// <param name="listener">监听者</param>
        /// <returns>状态</returns>
        public static int setListener(FCSocketListener listener) {
            m_listener = listener;
            return 1;
        }

        /// <summary>
        /// 写日志的回调
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="localSID">本地ID</param>
        /// <param name="state">状态</param>
        /// <param name="log">日志</param>
        public static void writeClientLog(int socketID, int localSID, int state, String log) {
            m_listener.writeLog(socketID, localSID, state, log);
        }
    }
}
