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
    /// 套接字监听
    /// </summary>
    public interface FCSocketListener {
        /// <summary>
        /// 数据回调
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="localSID">本地ID</param>
        /// <param name="str">数据</param>
        /// <param name="len">长度</param>
        void callBack(int socketID, int localSID, byte[] str, int len);
        /// <summary>
        /// 日志回调
        /// </summary>
        /// <param name="socketID">套接字ID</param>
        /// <param name="localSID">本地ID</param>
        /// <param name="state">状态</param>
        /// <param name="log">日志u</param>
        void writeLog(int socketID, int localSID, int state, String log);
    }
}
