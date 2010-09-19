/*
 * FCKeditor - The text editor for Internet - http://www.fckeditor.net
 * Copyright (C) 2003-2010 Frederico Caldeira Knabben
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 *
 * Contains the DTD mapping for XHTML 1.0 Transitional.
 * This file was automatically generated from the file: xhtml10-transitional.dtd
 */
FCK.DTD = (function()
{
    var X = FCKTools.Merge ;

    var A,L,J,M,N,O,D,H,P,K,Q,F,G,C,B,E,I ;
    A = {isindex:1, fieldset:1} ;
    B = {input:1, button:1, select:1, textarea:1, label:1} ;
    C = X({a:1}, B) ;
    D = X({iframe:1}, C) ;
    E = {hr:1, ul:1, menu:1, div:1, blockquote:1, noscript:1, table:1, center:1, address:1, dir:1, pre:1, h5:1, dl:1, h4:1, noframes:1, h6:1, ol:1, h1:1, h3:1, h2:1} ;
    F = {ins:1, del:1, script:1} ;
    G = X({b:1, acronym:1, bdo:1, 'var':1, '#':1, abbr:1, code:1, br:1, i:1, cite:1, kbd:1, u:1, strike:1, s:1, tt:1, strong:1, q:1, samp:1, em:1, dfn:1, span:1}, F) ;
    H = X({sub:1, img:1, object:1, sup:1, basefont:1, map:1, applet:1, font:1, big:1, small:1}, G) ;
    I = X({p:1}, H) ;
    J = X({iframe:1}, H, B) ;
    K = {img:1, noscript:1, br:1, kbd:1, center:1, button:1, basefont:1, h5:1, h4:1, samp:1, h6:1, ol:1, h1:1, h3:1, h2:1, form:1, font:1, '#':1, select:1, menu:1, ins:1, abbr:1, label:1, code:1, table:1, script:1, cite:1, input:1, iframe:1, strong:1, textarea:1, noframes:1, big:1, small:1, span:1, hr:1, sub:1, bdo:1, 'var':1, div:1, object:1, sup:1, strike:1, dir:1, map:1, dl:1, applet:1, del:1, isindex:1, fieldset:1, ul:1, b:1, acronym:1, a:1, blockquote:1, i:1, u:1, s:1, tt:1, address:1, q:1, pre:1, p:1, em:1, dfn:1} ;

    L = X({a:1}, J) ;
    M = {tr:1} ;
    N = {'#':1} ;
    O = X({param:1}, K) ;
    P = X({form:1}, A, D, E, I) ;
    Q = {li:1} ;

    return {
        col: {},
        tr: {td:1, th:1},
        img: {},
        colgroup: {col:1},
        noscript: P,
        td: P,
        br: {},
        th: P,
        center: P,
        kbd: L,
        button: X(I, E),
        basefont: {},
        h5: L,
        h4: L,
        samp: L,
        h6: L,
        ol: Q,
        h1: L,
        h3: L,
        option: N,
        h2: L,
        form: X(A, D, E, I),
        select: {optgroup:1, option:1},
        font: J,		// Changed from L to J (see (1))
        ins: P,
        menu: Q,
        abbr: L,
        label: L,
        table: {thead:1, col:1, tbody:1, tr:1, colgroup:1, caption:1, tfoot:1},
        code: L,
        script: N,
        tfoot: M,
        cite: L,
        li: P,
        input: {},
        iframe: P,
        strong: J,		// Changed from L to J (see (1))
        textarea: N,
        noframes: P,
        big: J,			// Changed from L to J (see (1))
        small: J,		// Changed from L to J (see (1))
        span: J,		// Changed from L to J (see (1))
        hr: {},
        dt: L,
        sub: J,			// Changed from L to J (see (1))
        optgroup: {option:1},
        param: {},
        bdo: L,
        'var': J,		// Changed from L to J (see (1))
        div: P,
        object: O,
        sup: J,			// Changed from L to J (see (1))
        dd: P,
        strike: J,		// Changed from L to J (see (1))
        area: {},
        dir: Q,
        map: X({area:1, form:1, p:1}, A, F, E),
        applet: O,
        dl: {dt:1, dd:1},
        del: P,
        isindex: {},
        fieldset: X({legend:1}, K),
        thead: M,
        ul: Q,
        acronym: L,
        b: J,			// Changed from L to J (see (1))
        a: J,
        blockquote: P,
        caption: L,
        i: J,			// Changed from L to J (see (1))
        u: J,			// Changed from L to J (see (1))
        tbody: M,
        s: L,
        address: X(D, I),
        tt: J,			// Changed from L to J (see (1))
        legend: L,
        q: L,
        pre: X(G, C),
        p: L,
        em: J,			// Changed from L to J (see (1))
        dfn: L
    } ;
})() ;

/*
	Notes:
	(1) According to the DTD, many elements, like <b> accept <a> elements
	    inside of them. But, to produce better output results, we have manually
	    changed the map to avoid breaking the links on pieces, having
	    "<b>this is a </b><a><b>link</b> test</a>", instead of
	    "<b>this is a <a>link</a></b><a> test</a>".
*/
