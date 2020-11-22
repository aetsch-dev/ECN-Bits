package de.telekom.llcto.ecn_bits.android.lib;

/*-
 * Copyright © 2020
 *      mirabilos <t.glaser@tarent.de>
 * Licensor: Deutsche Telekom
 *
 * Provided that these terms and disclaimer and all copyright notices
 * are retained or reproduced in an accompanying document, permission
 * is granted to deal in this work without restriction, including un‐
 * limited rights to use, publicly perform, distribute, sell, modify,
 * merge, give away, or sublicence.
 *
 * This work is provided “AS IS” and WITHOUT WARRANTY of any kind, to
 * the utmost extent permitted by applicable law, neither express nor
 * implied; without malicious intent or gross negligence. In no event
 * may a licensor, author or contributor be held liable for indirect,
 * direct, other damage, loss, or other issues arising in any way out
 * of dealing in the work, even if advised of the possibility of such
 * damage or existence of a defect, except proven that it results out
 * of said person’s immediate fault when using the work as intended.
 */

import lombok.extern.java.Log;
import lombok.val;
import org.junit.Test;

import java.util.logging.Level;

import static junit.framework.TestCase.fail;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertThrows;

@Log
public class JNITest {
    @Test
    public void testClassBoots() {
        LOGGER.info("testing Java™ part of JNI class…");
        val ap = new JNI.AddrPort();
        ap.addr = new byte[4];
        ap.port = 666;
        LOGGER.info("it works: " + ap.get());
    }

    @Test
    public void testJNIBoots() {
        LOGGER.info("testing JNI part of JNI class…");
        final long tid;
        try {
            tid = JNI.gettid();
        } catch (Throwable t) {
            LOGGER.log(Level.SEVERE, "it failed", t);
            fail("JNI does not work");
            return;
        }
        LOGGER.info("it also works: " + tid);
    }

    @Test
    public void testSignallingThrows() {
        final JNI.ErrnoException t = assertThrows("want an ESRCH exception",
          JNI.ErrnoException.class, () -> JNI.sigtid(0));
        LOGGER.log(Level.INFO, "successfully caught", t);
        assertEquals("is not ESRCH", /* ESRCH */3, t.getErrno());
    }
}
