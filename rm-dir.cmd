if exist %1 (
	rd %1 /Q /S
	if exist %1 (
		echo cannot delete directory %1
		exit /b 1
	)
)