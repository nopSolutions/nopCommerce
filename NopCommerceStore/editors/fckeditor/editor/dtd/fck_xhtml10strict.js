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
 * Contains the DTD mapping for XHTML 1.0 Strict.
 * This file was automatically generated from the file: xhtml10-strict.dtd
 */
FCK.DTD = (function()
{
    var X = FCKTools.Merge ;

    var H,I,J,K,C,L,M,A,B,D,E,G,N,F ;
    A = {ins:1, del:1, script:1} ;
    B = {hr:1, ul:1, div:1, blockquote:1, noscript:1, table:1, address:1, pre:1, p:1, h5:1, dl:1, h4:1, ol:1, h6:1, h1:1, h3:1, h2:1} ;
    C = X({fieldset:1}, B) ;
    D = X({sub:1, bdo:1, 'var':1, sup:1, br:1, kbd:1, map:1, samp:1, b:1, acronym:1, '#':1, abbr:1, code:1, i:1, cite:1, tt:1, strong:1, q:1, em:1, big:1, small:1, span:1, dfn:1}, A) ;
    E = X({img:1, object:1}, D) ;
    F = {input:1, button:1, textarea:1, select:1, label:1} ;
    G = X({a:1}, F) ;
    H = {img:1, noscript:1, br:1, kbd:1, button:1, h5:1, h4:1, samp:1, h6:1, ol:1, h1:1, h3:1, h2:1, form:1, select:1, '#':1, ins:1, abbr:1, label:1, code:1, table:1, script:1, cite:1, input:1, strong:1, textarea:1, big:1, small:1, span:1, hr:1, sub:1, bdo:1, 'var':1, div:1, object:1, sup:1, map:1, dl:1, del:1, fieldset:1, ul:1, b:1, acronym:1, a:1, blockquote:1, i:1, address:1, tt:1, q:1, pre:1, p:1, em:1, dfn:1} ;

    I = X({form:1, fieldset:1}, B, E, G) ;
    J = {tr:1} ;
    K = {'#':1} ;
    L = X(E, G) ;
    M = {li:1} ;
    N = X({form:1}, A, C) ;

    return {
        col: {},
        tr: {td:1, th:1},
        img: {},
        colgroup: {col:1},
        noscript: N,
        td: I,
        br: {},
        th: I,
        kbd: L,
        button: X(B, E),
        h5: L,
        h4: L,
        samp: L,
        h6: L,
        ol: M,
        h1: L,
        h3: L,
        option: K,
        h2: L,
        form: X(A, C),
        select: {optgroup:1, option:1},
        ins: I,
        abbr: L,
        label: L,
        code: L,
        table: {thead:1, col:1, tbody:1, tr:1, colgroup:1, caption:1, tfoot:1},
        script: K,
        tfoot: J,
        cite: L,
        li: I,
        input: {},
        strong: L,
        textarea: K,
        big: L,
        small: L,
        span: L,
        dt: L,
        hr: {},
        sub: L,
        optgroup: {option:1},
        bdo: L,
        param: {},
        'var': L,
        div: I,
        object: X({param:1}, H),
        sup: L,
        dd: I,
        area: {},
        map: X({form:1, area:1}, A, C),
        dl: {dt:1, dd:1},
        del: I,
        fieldset: X({legend:1}, H),
        thead: J,
        ul: M,
        acronym: L,
        b: L,
        a: X({img:1, object:1}, D, F),
        blockquote: N,
        caption: L,
        i: L,
        tbody: J,
        address: L,
        tt: L,
        legend: L,
        q: L,
        pre: X({a:1}, D, F),
        p: L,
        em: L,
        dfn: L
    } ;
})() ;
