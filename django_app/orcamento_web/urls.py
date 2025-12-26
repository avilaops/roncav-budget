"""
URL configuration for orcamento_web project.
"""
from django.contrib import admin
from django.urls import path, include

urlpatterns = [
    path('admin/', admin.site.urls),
    path('', include('budget.urls')),
    path('api/', include('budget.api.urls')),
]
