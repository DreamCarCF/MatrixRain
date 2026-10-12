'use strict';
const prefix='matrix-rain-'+new URL(self.registration.scope).pathname+'-';
const cacheName=prefix+'v1';
const assets=['./','./index.html','./manifest.webmanifest','./assets/icon-192.png','./assets/icon-512.png'];
self.addEventListener('install',event=>{event.waitUntil(caches.open(cacheName).then(cache=>cache.addAll(assets)).then(()=>self.skipWaiting()));});
self.addEventListener('activate',event=>{event.waitUntil(caches.keys().then(keys=>Promise.all(keys.filter(key=>key.startsWith(prefix)&&key!==cacheName).map(key=>caches.delete(key)))).then(()=>self.clients.claim()));});
self.addEventListener('fetch',event=>{
  if(event.request.method!=='GET'||!event.request.url.startsWith(self.registration.scope))return;
  const allowed=new Set(assets.map(path=>new URL(path,self.registration.scope).pathname));
  if(!allowed.has(new URL(event.request.url).pathname))return;
  event.respondWith((async()=>{
    const cache=await caches.open(cacheName);
    try{const response=await fetch(event.request);if(response.ok)await cache.put(event.request,response.clone());return response;}
    catch(_){return (await cache.match(event.request,{ignoreSearch:true}))||new Response('Offline resource unavailable',{status:503});}
  })());
});
