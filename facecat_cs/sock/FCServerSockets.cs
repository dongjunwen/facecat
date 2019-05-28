using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FaceCat {
    /// <summary>
    /// 服务端套接字连接管理
    /// </summary>
    public class FCServerSockets {
        /// <summary>
        /// 监听者
        /// </summary>
        private static FCSocketListener m_listener;

        /// <summary>
        /// 服务端集合
        /// </summary>
        private static HashMap<int, FCServerSocket> m_servers = new HashMap<int, FCServerSocket>();

        /// <summary>
        /// 套接字ID
        /// </summary>
        private static int m_socketID;

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <returns>状态</returns>
        public static int close(int socketID) {
            int ret = -1;
            if (m_servers.containsKey(socketID)) {
                FCServerSocket server = m_servers.get(socketID);
                ret = server.close();
                m_servers.remove(socketID);
            }
            return ret;
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <returns>状态</returns>
        public static int closeClient(int socketID) {
            int ret = -1;
            if (m_servers.containsKey(socketID)) {
                FCServerSocket server = m_servers.get(socketID);
                ret = server.closeClient(socketID);
                m_servers.remove(socketID);
            }
            return ret;
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
        /// <param name="localSID">本地ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        public static int send(int socketID, int localSID, byte[] str, int len) {
            int ret = -1;
            if (m_servers.containsKey(localSID)) {
                FCServerSocket server = m_servers.get(localSID);
                ret = server.send(socketID, str, len);
            }
            return ret;
        }

        /// <summary>
        /// UDP发送
        /// </summary>
        /// <param name="localSID">本地ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        public static int sendTo(int localSID, byte[] str, int len) {
            return -1;
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
        /// 启动服务
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="port">端口</param>
        /// <returns>状态</returns>
        public static int start(int type, int port) {
            try {
                FCServerSocket server = new FCServerSocket();
                if (type == 0) {
                    server.startTCP(port);
                }
                m_socketID++;
                int socketID = m_socketID;
                server.m_hSocket = m_socketID;
                m_servers.put(socketID, server);
                return m_socketID;
            }
            catch (Exception ex) {
                return -1;
            }
        }

        public static void writeServerLog(int socketID, int localSID, int state, String log) {
            m_listener.writeLog(socketID, localSID, state, log);
        }
    }
}
