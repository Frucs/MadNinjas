import test from 'node:test';
import assert from 'node:assert/strict';
import { ApiAdapter } from '../WebHost/wwwroot/api-adapter.js';
const response = (status, data) => ({status, ok:status>=200&&status<300, json:async()=>data});
const state = {phase:'resolved',playerName:'Andreas',balance:120,rounds:1,playerCards:[],dealerCards:[]};

test('a failed save response reconciles the settled balance without repeating the payment', async () => {
 const original=globalThis.fetch; const calls=[];
 globalThis.fetch=async (path,options)=>{calls.push([path,options.method]);return path==='/api/state'?response(200,state):response(503,{error:'Could not save result.'});};
 try {
  const game=new ApiAdapter();
  await assert.rejects(game.stand(),/Could not save/);
  assert.equal(game.getState().balance,120);
  assert.equal(game.getState().phase,'resolved');
  assert.deepEqual(calls,[['/api/stand','POST'],['/api/state','GET']]);
 } finally {globalThis.fetch=original;}
});
test('an expired server session clears the obsolete playing state', async () => {
 const original=globalThis.fetch;
 globalThis.fetch=async()=>response(404,{error:'Session expired.'});
 try {
  const game=new ApiAdapter();game.state={...state,phase:'playing'};
  await assert.rejects(game.hit(),/Session expired/);
  assert.equal(game.getState().playerName,'');
  assert.equal(game.getState().phase,'idle');
 } finally {globalThis.fetch=original;}
});
test('a transport failure marks the connection offline and preserves its original error',async()=>{
 const original=globalThis.fetch;globalThis.fetch=async()=>{throw new Error('Network down');};
 try {
  const game=new ApiAdapter();game.connected=true;
  await assert.rejects(game.stand(),/Cannot connect/);
  assert.equal(game.connected,false);
 } finally {globalThis.fetch=original;}
});
test('successful reconnect restores authoritative session state',async()=>{
 const original=globalThis.fetch;globalThis.fetch=async()=>response(200,state);
 try {const game=new ApiAdapter();await game.load();assert.equal(game.connected,true);assert.equal(game.getState().balance,120);}
 finally {globalThis.fetch=original;}
});
