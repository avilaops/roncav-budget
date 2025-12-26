"""
WSGI config for orcamento_web project.
"""

import os

from django.core.wsgi import get_wsgi_application

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'orcamento_web.settings')

application = get_wsgi_application()
