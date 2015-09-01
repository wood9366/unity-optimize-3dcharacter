fn exportCharacterAnimationTo fps cam lookAngleA lookAngleBStep lookDistance intervals = (
	local outputdir = getSavePath caption:"Export character animation texture to"
	
	if outputdir != undefined then (
		frameRate = fps
		
		for lookAngleB = 0 to 359 by lookAngleBStep do (
			local sina = sin lookAngleA
			local cosa = cos lookAngleA
			local sinb = sin lookAngleB
			local cosb = cos lookAngleB
			local pos = [lookDistance * cosa * cosb, lookDistance * cosa * sinb, lookDistance * sina]
			
			cam.position = pos
			
			for inv in intervals do (
				for t = inv.start to inv.end by 1f do (
					at time t (
						local name = (getFileNameFile maxFileName) \ 
							+ "_" + (lookAngleA as string) \ 
							+ "_" + (lookAngleB as string) \ 
							+ "_" + ((t.frame as integer) as string)
						local path = outputdir + "\\" + name + ".png"
						render camera:cam outputwidth:320 outputheight:240 outputFile:path quiet:true vfb:false
					)
				)
			)
		)
	)
)

fn debugExportTo cam = (
	frameRate = 30
	local intervals = #()
	
	append intervals (interval 0f 60f)
	append intervals (interval 120f 165f)
	
	oldfps = frameRate
	exportCharacterAnimationTo 15 cam 45 30 40000 intervals
	frameRate = oldfps

)