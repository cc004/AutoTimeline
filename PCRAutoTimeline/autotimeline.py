import sys
from clr_loader import get_coreclr
from pythonnet import set_runtime

_runtime_config = "runtimeconfig.json"
_rt = get_coreclr(_runtime_config)
set_runtime(_rt)

import clr
clr.AddReference('PCRAutoTimeline')

from PCRAutoTimeline.Interaction import Autopcr as autopcr
from PCRAutoTimeline.Interaction import Input
from PCRAutoTimeline.Interaction import Async as _Async
from PCRAutoTimeline.Interaction import Minitouch as minitouch
from PCRAutoTimeline.Interaction import Monitor as monitor
from PCRAutoTimeline.Interaction import Program
import System;

class Async:
	@staticmethod
	def Await():
		_Async.Await()
	@staticmethod
	def Start(f):
		_Async.Start(System.Action(f))
	@staticmethod
	def Exit():
		_Async.Exit()
Program.Main()
