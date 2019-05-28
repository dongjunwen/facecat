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
using System.Runtime.InteropServices;
using System.Net;
using System.Threading;

namespace FaceCat {
    /// <summary>
    /// 服务端套接字
    /// </summary>
    public class FCServerSocket {
        /// <summary>
        /// 同步参数
        /// </summary>
        private SocketAsyncEventArgs m_args;
        /// <summary>
        /// 数据集合
        /// </summary>
        public ArrayList<SOCKDATA> m_datas = new ArrayList<SOCKDATA>();
        /// <summary>
        /// 套接字ID
        /// </summary>
        public int m_hSocket;
        /// <summary>
        /// 端口
        /// </summary>
        private int m_port;
        /// <summary>
        /// 套接字对象
        /// </summary>
        private Socket m_socket = null;

        /// <summary>
        /// 接收TCP的线程
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="e">参数</param>
        private unsafe void acceptHandleTCP(object sender, SocketAsyncEventArgs e) {
            try {
                Socket socket = e.AcceptSocket;
                e.AcceptSocket = null;
                SOCKDATA data = new SOCKDATA();
                data.m_socket = socket;
                int socketID = (int)socket.Handle;
                data.m_hSocket = socketID;
                m_datas[socketID] = data;
                String remoteIP = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                int remotePort = ((IPEndPoint)socket.RemoteEndPoint).Port;
                String szAccept = String.Format("accept:{0}:{1}", remoteIP, remotePort);
                FCServerSockets.writeServerLog((int)socketID, m_hSocket, 1, szAccept);
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(onIoCompleted);
                args.SetBuffer(new byte[1024], 0, 1024);
                args.AcceptSocket = socket;
                if (!socket.ReceiveAsync(args)) {
                    processReceive(args);
                }
                beginAccept(e);
            } catch (Exception ex) {
            }
        }

        /// <summary>
        /// 开始接收
        /// </summary>
        /// <param name="e">参数</param>
        private void beginAccept(SocketAsyncEventArgs e) {
            e.AcceptSocket = null;
            //异步操作完成，返回false
            if (!m_socket.AcceptAsync(e)) {
                acceptHandleTCP(m_socket, e);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns>状态</returns>
        public int close() {
            if (m_socket != null) {
                try {
                    m_socket.Close();
                    return 1;
                } catch (Exception ex) {

                }
            }
            return -1;
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <returns>状态</returns>
        public int closeClient(int socketID) {
            try {
                if (m_datas[socketID] != null) {
                    m_datas[socketID].m_socket.Close();
                    return 1;
                }
            } catch (Exception ex) {
            }
            return -1;
        }

        /// <summary>
        /// 完成端口完成事件
        /// </summary>
        /// <param name="sender">调用者</param>
        /// <param name="e">参数</param>
        private void onIoCompleted(object sender, SocketAsyncEventArgs e) {
            switch (e.LastOperation) {
                case SocketAsyncOperation.Receive:
                    processReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    processSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        /// <summary>
        /// 处理接收
        /// </summary>
        /// <param name="e">参数</param>
        private void processReceive(SocketAsyncEventArgs e) {
            try {
                if (e.BytesTransferred > 0) {
                    if (e.SocketError == SocketError.Success) {
                        byte[] buffer = e.Buffer;
                        int socketID = (int)e.AcceptSocket.Handle;
                        SOCKDATA data = m_datas[socketID];
                        if (data != null) {
                            data.m_buffer = e.Buffer;
                            data.m_len = e.BytesTransferred;
                            int state = recv(data);
                            if (state == -1) {
                                data.m_socket.Close();
                                FCServerSockets.writeServerLog((int)socketID, m_hSocket, 2, "socket exit");
                                m_datas[socketID] = null;
                                return;
                            }
                        }
                        if (!e.AcceptSocket.ReceiveAsync(e)) {
                            processReceive(e);
                        }
                    }
                } else {
                    int socketID2 = (int)e.AcceptSocket.Handle;
                    SOCKDATA data2 = m_datas[socketID2];
                    data2.m_socket.Close();
                    m_datas[socketID2] = null;
                    FCServerSockets.writeServerLog(socketID2, m_hSocket, 2, "socket exit");
                }
            } catch (Exception ex) {
            }
        }

        /// <summary>
        /// 处理发送
        /// </summary>
        /// <param name="e">参数</param>
        private void processSend(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                //接收完毕开始下次接收
                if (!e.AcceptSocket.ReceiveAsync(e)) {
                    this.processReceive(e);
                }
            } else {

            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>状态</returns>
        public int recv(SOCKDATA data) {
            if (!data.m_submit) {
                if (data.m_len == 1024 && data.m_buffer[0] == 'm' && data.m_buffer[1] == 'i' && data.m_buffer[2] == 'a' && data.m_buffer[3] == 'o') {
                    data.m_submit = true;
                    return 1;
                } else {
                    return -1;
                }
            }
            int intSize = 4;
            data.m_index = 0;
            while (data.m_index < data.m_len) {
                int diffSize = 0;
                if (!data.m_get) {
                    diffSize = intSize - data.m_headSize;
                    if (diffSize == 0) {
                        data.m_head = (((data.m_buffer[data.m_index] | (data.m_buffer[data.m_index + 1] << 8)) | (data.m_buffer[data.m_index + 2] << 0x10)) | (data.m_buffer[data.m_index + 3] << 0x18));
                    } else {
                        for (int i = 0; i < diffSize; i++) {
                            data.m_headStr[data.m_headSize + i] = data.m_buffer[i];
                        }
                        data.m_head = (((data.m_headStr[0] | (data.m_headStr[1] << 8)) | (data.m_headStr[2] << 0x10)) | (data.m_headStr[3] << 0x18));
                    }
                    if (data.m_str != null) {
                        data.m_str = null;
                    }
                    data.m_str = new byte[data.m_head];
                    if (diffSize > 0) {
                        for (int i = 0; i < data.m_headSize; i++) {
                            data.m_str[i] = data.m_headStr[i];
                        }
                        data.m_pos += data.m_headSize;
                        data.m_headSize = intSize;
                    }
                }
                data.m_bufferRemain = data.m_len - data.m_index;
                data.m_strRemain = data.m_head - data.m_pos;
                data.m_get = data.m_strRemain > data.m_bufferRemain;
                int remain = Math.Min(data.m_strRemain, data.m_bufferRemain);
                Array.Copy(data.m_buffer, data.m_index, data.m_str, data.m_pos, remain);
                data.m_pos += remain;
                data.m_index += remain;
                if (!data.m_get) {
                    FCServerSockets.recvClientMsg(data.m_hSocket, m_hSocket, data.m_str, data.m_head);
                    data.m_head = 0;
                    data.m_pos = 0;
                    if (data.m_len - data.m_index == 0 || data.m_len - data.m_index >= intSize) {
                        data.m_headSize = intSize;
                    } else {
                        data.m_headSize = data.m_bufferRemain - data.m_strRemain;
                        for (int j = 0; j < data.m_headSize; j++) {
                            data.m_headStr[j] = data.m_buffer[data.m_index + j];
                        }
                        break;
                    }
                }
            }
            return 1;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="str">字符串</param>
        /// <param name="len">长度</param>
        /// <returns>状态</returns>
        public int send(int socketID, byte[] str, int len) {
            lock (this) {
                if (m_socket == null) {
                    return -1;
                }
                try {
                    return m_datas[socketID].m_socket.Send(str);
                } catch (Exception ex) {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 开始TCP服务
        /// </summary>
        /// <param name="port">端口</param>
        /// <returns>状态</returns>
        public unsafe int startTCP(int port) {
            m_port = port;
            try {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Any, m_port);
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.Bind(ipe);
                m_socket.Listen(0);
                m_hSocket = (int)m_socket.Handle;
                m_args = new SocketAsyncEventArgs();
                m_args.Completed += new EventHandler<SocketAsyncEventArgs>(acceptHandleTCP);
                m_datas = new ArrayList<SOCKDATA>();
                for (int i = 0; i < 100000; i++) {
                    m_datas.add(null);
                }
                m_socket.AcceptAsync(m_args);
                return 1;
            } catch (Exception ex) {
            }
            return -1;
        }
    }
}
